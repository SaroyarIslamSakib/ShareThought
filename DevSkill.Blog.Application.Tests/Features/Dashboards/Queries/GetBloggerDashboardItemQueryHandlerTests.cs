using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Dashboards.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;
using System.Linq.Expressions;

namespace DevSkill.Blog.Application.Tests;

public class GetBloggerDashboardItemQueryHandlerTests
{
    private AutoMock _moq;
    private GetBloggerDashboardItemQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IPostRepository> _postRepositoryMock;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _moq = AutoMock.GetLoose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _moq?.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = _moq.Mock<IApplicationUnitOfWork>();
        _postRepositoryMock = _moq.Mock<IPostRepository>();

        _handler = _moq.Create<GetBloggerDashboardItemQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ReturnsAggregatedBloggerDashboardCounts()
    {
        // Arrange
        var blogId = Guid.NewGuid();

        var query = new GetBloggerDashboardItemQuery
        {
            BlogId = blogId
        };

        _postRepositoryMock
            .Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Post, bool>>>()))
            .ReturnsAsync((Expression<Func<Post, bool>> exp) =>
            {
                var compiled = exp.Compile();

                var published = new Post { BlogAreaId = blogId, IsPublished = true };
                var draft = new Post { BlogAreaId = blogId, IsPublished = false };

                if (compiled(published)) return 3;
                if (compiled(draft)) return 2;

                return 0;
            })
            .Verifiable();

        _postRepositoryMock
            .Setup(x => x.TotalLikeCountInBlogAsync(blogId))
            .ReturnsAsync(25)
            .Verifiable();

        _postRepositoryMock
            .Setup(x => x.TotalCommentCountInBlogAsync(blogId))
            .ReturnsAsync(40)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.TotalPublishedPost.ShouldBe(3),
            () => result.TotalDraftPost.ShouldBe(2),
            () => result.TotalLike.ShouldBe(25),
            () => result.TotalComment.ShouldBe(40),

            () => _postRepositoryMock.Verify(x =>
                x.TotalLikeCountInBlogAsync(blogId), Times.Once),

            () => _postRepositoryMock.Verify(x =>
                x.TotalCommentCountInBlogAsync(blogId), Times.Once)
        );
    }
}

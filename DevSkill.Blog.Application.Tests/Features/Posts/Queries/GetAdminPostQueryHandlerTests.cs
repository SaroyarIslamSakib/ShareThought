using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetAdminPostQueryHandlerTests
{
    private AutoMock _moq;
    private GetAdminPostQueryHandler _handler;

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

        _handler = _moq.Create<GetAdminPostQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsPagedAdminPosts()
    {
        // Arrange
        var query = new GetAdminPostQuery
        {
            PageIndex = 1,
            PageSize = 10,
            SearchText = "test",
            SortOrder = "desc"
        };

        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "Post 1" },
            new Post { Id = Guid.NewGuid(), Title = "Post 2" }
        };

        _postRepositoryMock
            .Setup(x => x.GetPagedAdminPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder))
            .ReturnsAsync((posts, 2, 2))
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.total.ShouldBe(2);
        result.totalDisplay.ShouldBe(2);
        result.Item1.Count.ShouldBe(2);
        result.Item1[0].Title.ShouldBe("Post 1");

        _postRepositoryMock.Verify(x =>
            x.GetPagedAdminPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder), Times.Once);
    }
}

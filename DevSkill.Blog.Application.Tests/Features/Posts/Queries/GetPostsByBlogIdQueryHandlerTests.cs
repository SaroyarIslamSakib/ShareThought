using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetPostsByBlogIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetPostsByBlogIdQueryHandler _handler;

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

        _handler = _moq.Create<GetPostsByBlogIdQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsPublishedPagedPosts()
    {
        // Arrange
        var blogId = Guid.NewGuid();

        var query = new GetPostsByBlogIdQuery
        {
            BlogId = blogId,
            PageIndex = 1,
            PageSize = 10,
            SearchText = "",
            SortOrder = "asc"
        };

        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "Post 1", IsPublished = true },
            new Post { Id = Guid.NewGuid(), Title = "Post 2", IsPublished = true }
        };

        _postRepositoryMock
            .Setup(x => x.GetPublishedPagedPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blogId))
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
            x.GetPublishedPagedPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blogId), Times.Once);
    }
}

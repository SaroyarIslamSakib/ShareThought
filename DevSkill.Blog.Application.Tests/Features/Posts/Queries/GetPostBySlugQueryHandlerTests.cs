using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetPostBySlugQueryHandlerTests
{
    private AutoMock _moq;
    private GetPostBySlugQueryHandler _handler;

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

        _handler = _moq.Create<GetPostBySlugQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_PostExists_ReturnsPost()
    {
        // Arrange
        var blogSlug = "tech-blog";
        var postSlug = "my-post";

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "My Post",
            Slug = postSlug
        };

        var query = new GetPostBySlugQuery
        {
            BlogSlug = blogSlug,
            PostSlug = postSlug
        };

        _postRepositoryMock
            .Setup(x => x.GetPostBySlugAsync(blogSlug, postSlug))
            .ReturnsAsync(post)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe("My Post");
        result.Slug.ShouldBe(postSlug);

        _postRepositoryMock.Verify(x =>
            x.GetPostBySlugAsync(blogSlug, postSlug), Times.Once);
    }

    [Test]
    public async Task Handle_PostDoesNotExist_ReturnsNull()
    {
        // Arrange
        var blogSlug = "unknown-blog";
        var postSlug = "unknown-post";

        var query = new GetPostBySlugQuery
        {
            BlogSlug = blogSlug,
            PostSlug = postSlug
        };

        _postRepositoryMock
            .Setup(x => x.GetPostBySlugAsync(blogSlug, postSlug))
            .ReturnsAsync((Post)null);

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeNull();

        _postRepositoryMock.Verify(x =>
            x.GetPostBySlugAsync(blogSlug, postSlug), Times.Once);
    }
}

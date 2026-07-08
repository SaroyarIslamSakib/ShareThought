using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetPostByIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetPostByIdQueryHandler _handler;

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

        _handler = _moq.Create<GetPostByIdQueryHandler>();
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
        var postId = Guid.NewGuid();

        var post = new Post
        {
            Id = postId,
            Title = "Test Post",
            Content = "Test Content"
        };

        var query = new GetPostByIdQuery
        {
            PostId = postId
        };

        _postRepositoryMock
            .Setup(x => x.GetPostWithCategoriesTagsAsync(postId))
            .ReturnsAsync(post)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(postId);
        result.Title.ShouldBe("Test Post");

        _postRepositoryMock.Verify(x =>
            x.GetPostWithCategoriesTagsAsync(postId), Times.Once);
    }

    [Test]
    public async Task Handle_PostDoesNotExist_ReturnsNull()
    {
        // Arrange
        var postId = Guid.NewGuid();

        var query = new GetPostByIdQuery
        {
            PostId = postId
        };

        _postRepositoryMock
            .Setup(x => x.GetPostWithCategoriesTagsAsync(postId))
            .ReturnsAsync((Post)null);

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        _postRepositoryMock.Verify(x =>
            x.GetPostWithCategoriesTagsAsync(postId), Times.Once);
    }
}

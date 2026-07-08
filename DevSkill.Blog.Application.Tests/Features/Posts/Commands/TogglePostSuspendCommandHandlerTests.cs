using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class TogglePostSuspendCommandHandlerTests
{
    private AutoMock _moq;
    private TogglePostSuspendCommandHandler _handler;

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

        _handler = _moq.Create<TogglePostSuspendCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidPost_TogglesSuspendAndReturnsId()
    {
        // Arrange
        var postId = Guid.NewGuid();

        var post = new Post
        {
            Id = postId,
            IsSuspended = false
        };

        var command = new TogglePostSuspendCommand
        {
            Id = postId,
            IsSuspended = true
        };

        _postRepositoryMock
            .Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(post);

        _postRepositoryMock
            .Setup(x => x.EditAsync(It.Is<Post>(p =>
                p.Id == postId &&
                p.IsSuspended == true)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(postId);
        post.IsSuspended.ShouldBeTrue();

        _postRepositoryMock.Verify(x => x.GetByIdAsync(postId), Times.Once);
        _postRepositoryMock.Verify(x => x.EditAsync(It.IsAny<Post>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_PostNotFound_ThrowsException()
    {
        // Arrange
        var postId = Guid.NewGuid();

        var command = new TogglePostSuspendCommand
        {
            Id = postId,
            IsSuspended = true
        };

        _postRepositoryMock
            .Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync((Post)null);

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

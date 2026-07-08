using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class DeletePostCommandHandlerTests
{
    private AutoMock _moq;
    private DeletePostCommandHandler _handler;

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

        _handler = _moq.Create<DeletePostCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidPostId_RemovesPostAndReturnsId()
    {
        // Arrange
        var postId = Guid.NewGuid();

        var command = new DeletePostCommand
        {
            PostId = postId
        };

        _postRepositoryMock
            .Setup(x => x.RemoveAsync(postId))
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

        _postRepositoryMock.Verify(x => x.RemoveAsync(postId), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }
}

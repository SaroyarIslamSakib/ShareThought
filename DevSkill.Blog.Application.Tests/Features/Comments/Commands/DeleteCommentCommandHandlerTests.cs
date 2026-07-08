using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Comments.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class DeleteCommentCommandHandlerTests
{
    private AutoMock _moq;
    private DeleteCommentCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ICommentRepository> _commentRepositoryMock;

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
        _commentRepositoryMock = _moq.Mock<ICommentRepository>();

        _handler = _moq.Create<DeleteCommentCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _commentRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommentId_SoftDeletesCommentAndReturnsId()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        var comment = new Comment
        {
            Id = commentId,
            IsDeleted = false,
            DeletedAt = null
        };

        var command = new DeleteCommentCommand
        {
            CommentId = commentId
        };

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId))
            .ReturnsAsync(comment)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBe(commentId),
            () => comment.IsDeleted.ShouldBeTrue(),
            () => comment.DeletedAt.ShouldNotBeNull(),
            () => comment.DeletedAt.Value.ShouldBeInRange(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(5)),

            () => _commentRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }

    [Test]
    public async Task Handle_CommentNotFound_ThrowsException()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        var command = new DeleteCommentCommand
        {
            CommentId = commentId
        };

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId))
            .ReturnsAsync((Comment)null);

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

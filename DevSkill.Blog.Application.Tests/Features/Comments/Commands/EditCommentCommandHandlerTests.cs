using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Comments.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Domain.Utilities;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class EditCommentCommandHandlerTests
{
    private AutoMock _moq;
    private EditCommentCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ICommentRepository> _commentRepositoryMock;
    private Mock<IUserService> _userServiceMock;
    private Mock<IServerTime> _serverTimeMock;

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
        _userServiceMock = _moq.Mock<IUserService>();
        _serverTimeMock = _moq.Mock<IServerTime>();

        _handler = _moq.Create<EditCommentCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _commentRepositoryMock?.Reset();
        _userServiceMock?.Reset();
        _serverTimeMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidOwner_UpdatesCommentAndReturnsDto()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var comment = new Comment
        {
            Id = commentId,
            UserId = userId,
            Content = "Old Content",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpvoteCount = 3,
            ParentId = null
        };

        var command = new EditCommentCommand
        {
            CommentId = commentId,
            UserId = userId,
            Content = "Updated Content"
        };

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId))
            .ReturnsAsync(comment)
            .Verifiable();

        _serverTimeMock
            .Setup(x => x.DateTime)
            .Returns(now)
            .Verifiable();

        _commentRepositoryMock
            .Setup(x => x.EditAsync(It.Is<Comment>(c =>
                c.Id == commentId &&
                c.Content == "Updated Content" &&
                c.UpdatedAt == now)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        _userServiceMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(new UserListDto
            {
                Id = userId,
                FullName = "Test User",
                Email = "test@test.com"
            })
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.id.ShouldBe(commentId),
            () => result.content.ShouldBe("Updated Content"),
            () => result.fullname.ShouldBe("Test User"),
            () => result.upvote_count.ShouldBe(3),
            () => result.modified.ShouldBe(now),
            () => result.user_id.ShouldBe(userId.ToString()),

            () => _commentRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once),
            () => _userServiceMock.VerifyAll()
        );
    }

    [Test]
    public async Task Handle_CommentNotFound_ThrowsException()
    {
        // Arrange
        var command = new EditCommentCommand
        {
            CommentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = "Updated"
        };

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(command.CommentId))
            .ReturnsAsync((Comment)null);

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_NotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        var comment = new Comment
        {
            Id = commentId,
            UserId = Guid.NewGuid()
        };

        var command = new EditCommentCommand
        {
            CommentId = commentId,
            UserId = Guid.NewGuid(),
            Content = "Updated"
        };

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId))
            .ReturnsAsync(comment);

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

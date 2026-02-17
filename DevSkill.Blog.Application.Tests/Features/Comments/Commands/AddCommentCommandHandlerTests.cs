using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Comments.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Domain.Utilities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class AddCommentCommandHandlerTests
{
    private AutoMock _moq;
    private AddCommentCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ICommentRepository> _commentRepositoryMock;
    private Mock<IServerTime> _serverTimeMock;
    private Mock<IUserService> _userServiceMock;

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
        _serverTimeMock = _moq.Mock<IServerTime>();
        _userServiceMock = _moq.Mock<IUserService>();

        _handler = _moq.Create<AddCommentCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _commentRepositoryMock?.Reset();
        _serverTimeMock?.Reset();
        _userServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_AddsCommentAndReturnsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var command = new AddCommentCommand
        {
            PostId = postId,
            ParentId = null,
            UserId = userId,
            Content = "Nice post!"
        };

        _serverTimeMock
            .Setup(x => x.DateTime)
            .Returns(now)
            .Verifiable();

        _commentRepositoryMock
            .Setup(x => x.AddAsync(It.Is<Comment>(c =>
                c.PostId == postId &&
                c.UserId == userId &&
                c.Content == command.Content &&
                c.CreatedAt == now &&
                c.UpvoteCount == 0 &&
                c.Id != Guid.Empty)))
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
            () => result.ShouldNotBeNull(),
            () => result.content.ShouldBe(command.Content),
            () => result.fullname.ShouldBe("Test User"),
            () => result.created.ShouldBe(now),
            () => result.upvote_count.ShouldBe(0),
            () => result.user_has_upvoted.ShouldBeFalse(),

            () => _commentRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.VerifyAll(),
            () => _userServiceMock.VerifyAll()
        );
    }

}

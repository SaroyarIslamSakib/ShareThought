using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Comments.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class MarkCommentAsApprovedCommandHandlerTests
{
    private AutoMock _moq;
    private MarkCommentAsApprovedCommandHandler _handler;

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

        _handler = _moq.Create<MarkCommentAsApprovedCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _commentRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommentId_MarksAsApprovedAndReturnsId()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        var comment = new Comment
        {
            Id = commentId,
            IsApproved = false
        };

        var command = new MarkCommentAsApprovedCommand
        {
            Id = commentId
        };

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId))
            .ReturnsAsync(comment)
            .Verifiable();

        _commentRepositoryMock
            .Setup(x => x.EditAsync(It.Is<Comment>(c =>
                c.Id == commentId &&
                c.IsApproved == true)))
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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBe(commentId),
            () => comment.IsApproved.ShouldBeTrue(),

            () => _commentRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

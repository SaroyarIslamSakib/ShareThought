using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Comments.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetCommentsByPostIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetCommentsByPostIdQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ICommentRepository> _commentRepositoryMock;
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
        _userServiceMock = _moq.Mock<IUserService>();

        _handler = _moq.Create<GetCommentsByPostIdQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _commentRepositoryMock?.Reset();
        _userServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsFilteredAndMappedComments()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var currentUserId = user1;

        var query = new GetCommentsByPostIdQuery
        {
            PostId = postId,
            CurrentUserId = currentUserId
        };

        var comments = new List<Comment>
        {
            new Comment
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = user1,
                Content = "First comment",
                CreatedAt = DateTime.UtcNow,
                UpvoteCount = 2,
                IsDeleted = false,
                UpdatedAt = null
            },
            new Comment
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = user2,
                Content = "Deleted comment",
                CreatedAt = DateTime.UtcNow,
                UpvoteCount = 0,
                IsDeleted = true
            }
        };

        _commentRepositoryMock
            .Setup(x => x.GetByPostIdAsync(postId))
            .ReturnsAsync(comments)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object)
            .Verifiable();

        _userServiceMock
            .Setup(x => x.GetUserByIdAsync(user1))
            .ReturnsAsync(new UserListDto
            {
                Id = user1,
                FullName = "User One"
            })
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.Count.ShouldBe(1),
            () => result[0].content.ShouldBe("First comment"),
            () => result[0].fullname.ShouldBe("User One"),
            () => result[0].upvote_count.ShouldBe(2),
            () => result[0].created_by_current_user.ShouldBeTrue(),
            () => result[0].user_id.ShouldBe(user1.ToString()),

            () => _commentRepositoryMock.VerifyAll(),
            () => _userServiceMock.Verify(x => x.GetUserByIdAsync(user1), Times.Once)
        );
    }
}

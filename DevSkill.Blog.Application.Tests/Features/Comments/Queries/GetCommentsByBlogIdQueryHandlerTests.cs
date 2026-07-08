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

public class GetCommentsByBlogIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetCommentsByBlogIdQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
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
        _blogAreaRepositoryMock = _moq.Mock<IBlogAreaRepository>();
        _commentRepositoryMock = _moq.Mock<ICommentRepository>();
        _userServiceMock = _moq.Mock<IUserService>();

        _handler = _moq.Create<GetCommentsByBlogIdQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _commentRepositoryMock?.Reset();
        _userServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsMappedBlogCommentDtos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();

        var blog = new BlogArea
        {
            Id = blogId,
            UserId = userId
        };

        var query = new GetCommentsByBlogIdQuery
        {
            UserId = userId,
            PageIndex = 1,
            PageSize = 10,
            SearchText = "",
            SortOrder = "asc"
        };

        var commentId = Guid.NewGuid();

        var comments = new List<Comment>
        {
            new Comment
            {
                Id = commentId,
                Content = "Nice blog!",
                CreatedAt = DateTime.UtcNow,
                ParentId = null,
                IsApproved = true,
                Post = new Post
                {
                    Title = "Test Post"
                }
            }
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog })
            .Verifiable();

        _commentRepositoryMock
            .Setup(x => x.GetPagedBlogCommentsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blogId))
            .ReturnsAsync((comments, 1, 1))
            .Verifiable();

        _userServiceMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(new UserListDto
            {
                Id = userId,
                FullName = "Blog Owner",
                Email = "owner@test.com"
            })
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.total.ShouldBe(1),
            () => result.totalDisplay.ShouldBe(1),
            () => result.Item1.Count.ShouldBe(1),
            () => result.Item1[0].Content.ShouldBe("Nice blog!"),
            () => result.Item1[0].UserName.ShouldBe("Blog Owner"),
            () => result.Item1[0].PostTitle.ShouldBe("Test Post"),
            () => result.Item1[0].IsApproved.ShouldBeTrue(),

            () => _blogAreaRepositoryMock.VerifyAll(),
            () => _commentRepositoryMock.VerifyAll(),
            () => _userServiceMock.VerifyAll()
        );
    }

    [Test]
    public async Task Handle_BlogNotFound_ThrowsException()
    {
        // Arrange
        var query = new GetCommentsByBlogIdQuery
        {
            UserId = Guid.NewGuid(),
            PageIndex = 1,
            PageSize = 10
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(query.UserId))
            .ReturnsAsync(new List<BlogArea>());

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}

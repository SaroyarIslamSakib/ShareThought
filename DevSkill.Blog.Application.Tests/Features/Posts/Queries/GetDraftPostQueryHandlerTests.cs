using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetDraftPostQueryHandlerTests
{
    private AutoMock _moq;
    private GetDraftPostQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
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
        _blogAreaRepositoryMock = _moq.Mock<IBlogAreaRepository>();
        _postRepositoryMock = _moq.Mock<IPostRepository>();

        _handler = _moq.Create<GetDraftPostQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsPagedDraftPosts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();

        var blog = new BlogArea
        {
            Id = blogId,
            UserId = userId
        };

        var query = new GetDraftPostQuery
        {
            UserId = userId,
            PageIndex = 1,
            PageSize = 5,
            SearchText = "",
            SortOrder = "asc"
        };

        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "Draft 1", IsPublished = false },
            new Post { Id = Guid.NewGuid(), Title = "Draft 2", IsPublished = false }
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog });

        _postRepositoryMock
            .Setup(x => x.GetPagedDraftPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blogId))
            .ReturnsAsync((posts, 2, 2))
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.total.ShouldBe(2);
        result.totalDisplay.ShouldBe(2);
        result.Item1.Count.ShouldBe(2);
        result.Item1[0].Title.ShouldBe("Draft 1");

        _postRepositoryMock.Verify(x =>
            x.GetPagedDraftPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blogId), Times.Once);
    }

    [Test]
    public async Task Handle_BlogNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var query = new GetDraftPostQuery
        {
            UserId = userId,
            PageIndex = 1,
            PageSize = 5
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea>());

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}

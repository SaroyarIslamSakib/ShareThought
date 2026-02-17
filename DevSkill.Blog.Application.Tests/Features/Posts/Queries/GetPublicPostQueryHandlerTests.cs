using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetPublicPostQueryHandlerTests
{
    private AutoMock _moq;
    private GetPublicPostQueryHandler _handler;

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

        _handler = _moq.Create<GetPublicPostQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsPagedPublicPosts()
    {
        // Arrange
        var query = new GetPublicPostQuery
        {
            PageIndex = 1,
            PageSize = 10,
            SearchText = "asp",
            SortOrder = "asc",
            CategoryName = "Programming"
        };

        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "ASP.NET Core", IsPublished = true },
            new Post { Id = Guid.NewGuid(), Title = "C# Basics", IsPublished = true }
        };

        _postRepositoryMock
            .Setup(x => x.GetPagedPublicPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                query.CategoryName))
            .ReturnsAsync((posts, 2, 2))
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.total.ShouldBe(2);
        result.totalDisplay.ShouldBe(2);
        result.Item1.Count.ShouldBe(2);
        result.Item1[0].Title.ShouldBe("ASP.NET Core");

        _postRepositoryMock.Verify(x =>
            x.GetPagedPublicPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                query.CategoryName), Times.Once);
    }
}

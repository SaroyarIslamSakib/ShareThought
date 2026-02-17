using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetBlogAreaByUserIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetBlogAreaByUserIdQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;

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

        _handler = _moq.Create<GetBlogAreaByUserIdQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_BlogsExist_ReturnsFirstBlog()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var blogs = new List<BlogArea>
        {
            new BlogArea
            {
                Id = Guid.NewGuid(),
                Name = "First Blog",
                UserId = userId
            },
            new BlogArea
            {
                Id = Guid.NewGuid(),
                Name = "Second Blog",
                UserId = userId
            }
        };

        var query = new GetBlogAreaByUserIdQuery
        {
            UserId = userId
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(blogs)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(blogs.First().Id);
        result.Name.ShouldBe("First Blog");

        _blogAreaRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task Handle_NoBlogsExist_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var query = new GetBlogAreaByUserIdQuery
        {
            UserId = userId
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea>());

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        _blogAreaRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }
}

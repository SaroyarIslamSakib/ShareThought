using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetBlogAreaBySlugQueryHandlerTests
{
    private AutoMock _moq;
    private GetBlogAreaBySlugQueryHandler _handler;

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

        _handler = _moq.Create<GetBlogAreaBySlugQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_BlogExists_ReturnsBlog()
    {
        // Arrange
        var slug = "tech-blog";

        var blog = new BlogArea
        {
            Id = Guid.NewGuid(),
            Name = "Tech Blog",
            Slug = slug,
            IsSuspended = false
        };

        var query = new GetBlogAreaBySlugQuery
        {
            Slug = slug
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetBlogBySlug(slug))
            .ReturnsAsync(blog)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(blog.Id);
        result.Slug.ShouldBe(slug);

        _blogAreaRepositoryMock.Verify(x => x.GetBlogBySlug(slug), Times.Once);
    }

    [Test]
    public async Task Handle_BlogDoesNotExist_ReturnsNull()
    {
        // Arrange
        var slug = "unknown-blog";

        var query = new GetBlogAreaBySlugQuery
        {
            Slug = slug
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetBlogBySlug(slug))
            .ReturnsAsync((BlogArea)null);

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        _blogAreaRepositoryMock.Verify(x => x.GetBlogBySlug(slug), Times.Once);
    }
}

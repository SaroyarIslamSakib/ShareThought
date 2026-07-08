using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Categories.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetCategoriesQueryHandlerTests
{
    private AutoMock _moq;
    private GetCategoriesQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;

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
        _categoryRepositoryMock = _moq.Mock<ICategoryRepository>();

        _handler = _moq.Create<GetCategoriesQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _categoryRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidSearchTerm_ReturnsMatchingCategories()
    {
        // Arrange
        var query = new GetCategoriesQuery
        {
            SearchTerm = "tech"
        };

        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Technology" },
            new Category { Id = Guid.NewGuid(), Name = "Tech News" }
        };

        _categoryRepositoryMock
            .Setup(x => x.SearchByNameAsync(query.SearchTerm))
            .ReturnsAsync(categories)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.CategoryRepository)
            .Returns(_categoryRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Technology");

        _categoryRepositoryMock.Verify(x =>
            x.SearchByNameAsync(query.SearchTerm), Times.Once);
    }

    [Test]
    public async Task Handle_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCategoriesQuery
        {
            SearchTerm = "unknown"
        };

        _categoryRepositoryMock
            .Setup(x => x.SearchByNameAsync(query.SearchTerm))
            .ReturnsAsync(new List<Category>());

        _unitOfWorkMock
            .SetupGet(x => x.CategoryRepository)
            .Returns(_categoryRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);

        _categoryRepositoryMock.Verify(x =>
            x.SearchByNameAsync(query.SearchTerm), Times.Once);
    }
}

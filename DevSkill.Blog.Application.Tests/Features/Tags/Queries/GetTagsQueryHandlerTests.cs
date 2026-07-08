using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Tags.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetTagsQueryHandlerTests
{
    private AutoMock _moq;
    private GetTagsQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ITagRepository> _tagRepositoryMock;

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
        _tagRepositoryMock = _moq.Mock<ITagRepository>();

        _handler = _moq.Create<GetTagsQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _tagRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidSearchTerm_ReturnsMatchingTags()
    {
        // Arrange
        var query = new GetTagsQuery
        {
            SearchTerm = "asp"
        };

        var tags = new List<Tag>
        {
            new Tag { Id = Guid.NewGuid(), Name = "ASP.NET" },
            new Tag { Id = Guid.NewGuid(), Name = "ASP.NET Core" }
        };

        _tagRepositoryMock
            .Setup(x => x.SearchByNameAsync(query.SearchTerm))
            .ReturnsAsync(tags)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.TagRepository)
            .Returns(_tagRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.Count.ShouldBe(2),
            () => result[0].Name.ShouldBe("ASP.NET"),

            () => _tagRepositoryMock.VerifyAll()
        );
    }

    [Test]
    public async Task Handle_NoMatch_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetTagsQuery
        {
            SearchTerm = "unknown"
        };

        _tagRepositoryMock
            .Setup(x => x.SearchByNameAsync(query.SearchTerm))
            .ReturnsAsync(new List<Tag>())
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.TagRepository)
            .Returns(_tagRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.Count.ShouldBe(0),

            () => _tagRepositoryMock.VerifyAll()
        );
    }
}

using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetContactMessageQueryHandlerTests
{
    private AutoMock _moq;
    private GetContactMessageQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IContactMessageRepository> _contactRepositoryMock;

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
        _contactRepositoryMock = _moq.Mock<IContactMessageRepository>();

        _handler = _moq.Create<GetContactMessageQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsPagedContactMessages()
    {
        // Arrange
        var query = new GetContactMessageQuery
        {
            PageIndex = 1,
            PageSize = 10,
            SearchText = "john",
            SortOrder = "desc"
        };

        var messages = new List<ContactMessage>
        {
            new ContactMessage
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john@test.com"
            },
            new ContactMessage
            {
                Id = Guid.NewGuid(),
                Name = "Johnny",
                Email = "johnny@test.com"
            }
        };

        _contactRepositoryMock
            .Setup(x => x.GetPagedContactMessagesAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder))
            .ReturnsAsync((messages, 2, 2))
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.ContactMessageRepository)
            .Returns(_contactRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.total.ShouldBe(2),
            () => result.totalDisplay.ShouldBe(2),
            () => result.Item1.Count.ShouldBe(2),
            () => result.Item1[0].Name.ShouldBe("John Doe"),

            () => _contactRepositoryMock.VerifyAll()
        );
    }
}

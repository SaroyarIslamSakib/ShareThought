using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetContactMessageByIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetContactMessageByIdQueryHandler _handler;

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

        _handler = _moq.Create<GetContactMessageByIdQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_MessageExists_ReturnsContactMessage()
    {
        // Arrange
        var id = Guid.NewGuid();

        var message = new ContactMessage
        {
            Id = id,
            Name = "John",
            Email = "john@test.com"
        };

        var query = new GetContactMessageByIdQuery
        {
            Id = id
        };

        _contactRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(message)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.ContactMessageRepository)
            .Returns(_contactRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await ((Cortex.Mediator.Queries.IQueryHandler<GetContactMessageByIdQuery, ContactMessage>)_handler)
            .Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.Id.ShouldBe(id),
            () => result.Name.ShouldBe("John"),

            () => _contactRepositoryMock.VerifyAll()
        );
    }

    [Test]
    public async Task Handle_MessageNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        var query = new GetContactMessageByIdQuery
        {
            Id = id
        };

        _contactRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((ContactMessage)null)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.ContactMessageRepository)
            .Returns(_contactRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await ((Cortex.Mediator.Queries.IQueryHandler<GetContactMessageByIdQuery, ContactMessage>)_handler)
            .Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBeNull(),
            () => _contactRepositoryMock.VerifyAll()
        );
    }
}

using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Enums;
using DevSkill.Blog.Domain.Repositories;
using MapsterMapper;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class ContactMessageAddCommandHandlerTests
{
    private AutoMock _moq;
    private ContactMessageAddCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IContactMessageRepository> _contactRepositoryMock;
    private Mock<IMapper> _mapperMock;

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
        _mapperMock = _moq.Mock<IMapper>();

        _handler = _moq.Create<ContactMessageAddCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
        _mapperMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_AddsContactMessageAndReturnsEntity()
    {
        // Arrange
        var command = new ContactMessageAddCommand
        {
            Name = "John Doe",
            Email = "john@test.com",
            Topic = ContactTopic.Copyright,
            Message = "Hello there!"
        };

        var mappedMessage = new ContactMessage
        {
            Name = command.Name,
            Email = command.Email,
            Topic = command.Topic,
            Message = command.Message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            Status = MessageStatus.Pending
        };

        _mapperMock
            .Setup(x => x.Map<ContactMessage>(command))
            .Returns(mappedMessage)
            .Verifiable();

        _contactRepositoryMock
            .Setup(x => x.AddAsync(It.Is<ContactMessage>(m =>
                m.Name == command.Name &&
                m.Email == command.Email &&
                m.Topic == command.Topic &&
                m.Message == command.Message &&
                m.Id != Guid.Empty)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.ContactMessageRepository)
            .Returns(_contactRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.Name.ShouldBe(command.Name),
            () => result.Email.ShouldBe(command.Email),
            () => result.Topic.ShouldBe(command.Topic),
            () => result.Message.ShouldBe(command.Message),
            () => result.Id.ShouldNotBe(Guid.Empty),

            () => _mapperMock.VerifyAll(),
            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

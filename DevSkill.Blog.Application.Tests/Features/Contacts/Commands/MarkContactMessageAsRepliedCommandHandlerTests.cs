using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Enums;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class MarkContactMessageAsRepliedCommandHandlerTests
{
    private AutoMock _moq;
    private MarkContactMessageAsRepliedCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IContactMessageRepository> _contactRepositoryMock;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _moq = AutoMock.GetLoose();
    }

    [OneTimeSetUp]
    public void Init()
    {
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

        _handler = _moq.Create<MarkContactMessageAsRepliedCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_StatusNotReplied_SetsStatusToReplied()
    {
        // Arrange
        var id = Guid.NewGuid();

        var message = new ContactMessage
        {
            Id = id,
            Status = MessageStatus.Pending
        };

        var command = new MarkContactMessageAsRepliedCommand
        {
            Id = id
        };

        _contactRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(message)
            .Verifiable();

        _contactRepositoryMock
            .Setup(x => x.EditAsync(It.Is<ContactMessage>(m =>
                m.Id == id &&
                m.Status == MessageStatus.Replied)))
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
            () => result.ShouldBe(id),
            () => message.Status.ShouldBe(MessageStatus.Replied),

            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }

    [Test]
    public async Task Handle_AlreadyReplied_KeepsStatusReplied()
    {
        // Arrange
        var id = Guid.NewGuid();

        var message = new ContactMessage
        {
            Id = id,
            Status = MessageStatus.Replied
        };

        var command = new MarkContactMessageAsRepliedCommand
        {
            Id = id
        };

        _contactRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(message)
            .Verifiable();

        _contactRepositoryMock
            .Setup(x => x.EditAsync(It.Is<ContactMessage>(m =>
                m.Id == id &&
                m.Status == MessageStatus.Replied)))
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
            () => result.ShouldBe(id),
            () => message.Status.ShouldBe(MessageStatus.Replied),

            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

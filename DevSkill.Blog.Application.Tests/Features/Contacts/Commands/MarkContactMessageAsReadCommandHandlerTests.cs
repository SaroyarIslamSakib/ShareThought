using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class MarkContactMessageAsReadCommandHandlerTests
{
    private AutoMock _moq;
    private MarkContactMessageAsReadCommandHandler _handler;

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

        _handler = _moq.Create<MarkContactMessageAsReadCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_MessageNotRead_MarksAsReadAndReturnsId()
    {
        // Arrange
        var id = Guid.NewGuid();

        var message = new ContactMessage
        {
            Id = id,
            IsRead = false
        };

        var command = new MarkContactMessageAsReadCommand
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
                m.IsRead == true)))
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
            () => message.IsRead.ShouldBeTrue(),

            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }

    [Test]
    public async Task Handle_MessageAlreadyRead_KeepsTrueAndReturnsId()
    {
        // Arrange
        var id = Guid.NewGuid();

        var message = new ContactMessage
        {
            Id = id,
            IsRead = true
        };

        var command = new MarkContactMessageAsReadCommand
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
                m.IsRead == true)))
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
            () => message.IsRead.ShouldBeTrue(),

            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

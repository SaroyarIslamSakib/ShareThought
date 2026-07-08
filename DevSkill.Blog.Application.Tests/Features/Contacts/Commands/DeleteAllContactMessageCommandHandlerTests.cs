using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class DeleteAllContactMessageCommandHandlerTests
{
    private AutoMock _moq;
    private DeleteAllContactMessageCommandHandler _handler;

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

        _handler = _moq.Create<DeleteAllContactMessageCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_Command_RemovesAllMessagesAndSaves()
    {
        // Arrange
        var command = new DeleteAllContactMessageCommand();

        _contactRepositoryMock
            .Setup(x => x.RemoveAllAsync())
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
        var resultTask = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => resultTask.ShouldNotBeNull(),
            () => resultTask.ShouldBe(Task.CompletedTask),

            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class DeleteContactMessageCommandHandlerTests
{
    private AutoMock _moq;
    private DeleteContactMessageCommandHandler _handler;

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

        _handler = _moq.Create<DeleteContactMessageCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _contactRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidId_RemovesMessageAndReturnsId()
    {
        // Arrange
        var id = Guid.NewGuid();

        var command = new DeleteContactMessageCommand
        {
            Id = id
        };

        _contactRepositoryMock
            .Setup(x => x.RemoveAsync(id))
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

            () => _contactRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.SystemSettings.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class AddSettingsCommandHandlerTests
{
    private AutoMock _moq;
    private AddSettingsCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ISettingsRepository> _settingsRepositoryMock;

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
        _settingsRepositoryMock = _moq.Mock<ISettingsRepository>();

        _handler = _moq.Create<AddSettingsCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _settingsRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_AddsSettingsAndReturnsId()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow;

        var command = new AddSettingsCommand
        {
            TermsContent = "Sample Terms",
            StorageType = "Local",
            UpdatedAt = updatedAt
        };

        _settingsRepositoryMock
            .Setup(x => x.AddAsync(It.Is<Settings>(s =>
                s.TermsContent == command.TermsContent &&
                s.StorageType == command.StorageType &&
                s.UpdatedAt == updatedAt &&
                s.Id != Guid.Empty)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.SettingsRepository)
            .Returns(_settingsRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBe(Guid.Empty),

            () => _settingsRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.SystemSettings.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class UpdateSettingsCommandHandlerTests
{
    private AutoMock _moq;
    private UpdateSettingsCommandHandler _handler;

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

        _handler = _moq.Create<UpdateSettingsCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _settingsRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_SettingsExists_UpdatesAndReturnsSettings()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updatedAt = DateTime.UtcNow;

        var existing = new Settings
        {
            Id = id,
            TermsContent = "Old Terms",
            StorageType = "Local",
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateSettingsCommand
        {
            OldSettingsId = id,
            TermsContent = "New Terms",
            StorageType = "Cloud",
            UpdatedAt = updatedAt
        };

        _settingsRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(existing)
            .Verifiable();

        _settingsRepositoryMock
            .Setup(x => x.EditAsync(It.Is<Settings>(s =>
                s.Id == id &&
                s.TermsContent == command.TermsContent &&
                s.StorageType == command.StorageType &&
                s.UpdatedAt == updatedAt)))
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
            () => result.ShouldNotBeNull(),
            () => result.Id.ShouldBe(id),
            () => result.TermsContent.ShouldBe("New Terms"),
            () => result.StorageType.ShouldBe("Cloud"),
            () => result.UpdatedAt.ShouldBe(updatedAt),

            () => _settingsRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }

    [Test]
    public async Task Handle_SettingsNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        var command = new UpdateSettingsCommand
        {
            OldSettingsId = id,
            TermsContent = "New Terms",
            StorageType = "Cloud",
            UpdatedAt = DateTime.UtcNow
        };

        _settingsRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Settings)null)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.SettingsRepository)
            .Returns(_settingsRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBeNull(),
            () => _settingsRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never)
        );
    }
}

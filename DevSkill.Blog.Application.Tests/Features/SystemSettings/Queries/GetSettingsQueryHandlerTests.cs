using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.SystemSettings.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetSettingsQueryHandlerTests
{
    private AutoMock _moq;
    private GetSettingsQueryhandler _handler;

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

        _handler = _moq.Create<GetSettingsQueryhandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _settingsRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_SettingsExist_ReturnsFirstSettings()
    {
        // Arrange
        var settingsList = new List<Settings>
        {
            new Settings
            {
                Id = Guid.NewGuid(),
                TermsContent = "Terms 1",
                StorageType = "Local"
            },
            new Settings
            {
                Id = Guid.NewGuid(),
                TermsContent = "Terms 2",
                StorageType = "Cloud"
            }
        };

        _settingsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(settingsList)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.SettingsRepository)
            .Returns(_settingsRepositoryMock.Object)
            .Verifiable();

        var query = new GetSettingsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.TermsContent.ShouldBe("Terms 1"),

            () => _settingsRepositoryMock.VerifyAll()
        );
    }

    [Test]
    public async Task Handle_NoSettings_ReturnsNull()
    {
        // Arrange
        _settingsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync((IList<Settings>)null)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.SettingsRepository)
            .Returns(_settingsRepositoryMock.Object)
            .Verifiable();

        var query = new GetSettingsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBeNull(),
            () => _settingsRepositoryMock.VerifyAll()
        );
    }
}

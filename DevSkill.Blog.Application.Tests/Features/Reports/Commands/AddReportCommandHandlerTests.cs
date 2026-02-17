using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Reports.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Enums;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class AddReportCommandHandlerTests
{
    private AutoMock _moq;
    private AddReportCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IReportRepository> _reportRepositoryMock;

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
        _reportRepositoryMock = _moq.Mock<IReportRepository>();

        _handler = _moq.Create<AddReportCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _reportRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_AddsReportAndReturnsId()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var command = new AddReportCommand
        {
            PostId = postId,
            UserId = "userId",
            Reason = ReportReason.Spam,
            CustomReason = "Repeated content",
            CreatedAt = createdAt
        };

        _reportRepositoryMock
            .Setup(x => x.AddAsync(It.Is<Report>(r =>
                r.PostId == postId &&
                r.UserId == "userId" &&
                r.Reason == command.Reason &&
                r.CustomReason == command.CustomReason &&
                r.CreatedAt == createdAt &&
                r.Id != Guid.Empty)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.ReportRepository)
            .Returns(_reportRepositoryMock.Object)
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

            () => _reportRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

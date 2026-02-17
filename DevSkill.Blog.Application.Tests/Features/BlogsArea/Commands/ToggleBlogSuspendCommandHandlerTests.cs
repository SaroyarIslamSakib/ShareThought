using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class ToggleBlogSuspendCommandHandlerTests
{
    private AutoMock _moq;
    private ToggleBlogSuspendCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;

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
        _blogAreaRepositoryMock = _moq.Mock<IBlogAreaRepository>();

        _handler = _moq.Create<ToggleBlogSuspendCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidId_TogglesSuspendAndReturnsId()
    {
        // Arrange
        var blogId = Guid.NewGuid();

        var blog = new BlogArea
        {
            Id = blogId,
            Name = "Test Blog",
            IsSuspended = false
        };

        var command = new ToggleBlogSuspendCommand
        {
            Id = blogId,
            IsSuspended = true
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByIdAsync(blogId))
            .ReturnsAsync(blog)
            .Verifiable();

        _blogAreaRepositoryMock
            .Setup(x => x.EditAsync(It.Is<BlogArea>(b =>
                b.Id == blogId &&
                b.IsSuspended == true)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBe(blogId),
            () => blog.IsSuspended.ShouldBeTrue(),
            () => _blogAreaRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }

    [Test]
    public async Task Handle_BlogNotFound_ThrowsException()
    {
        // Arrange
        var blogId = Guid.NewGuid();

        var command = new ToggleBlogSuspendCommand
        {
            Id = blogId,
            IsSuspended = true
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByIdAsync(blogId))
            .ReturnsAsync((BlogArea)null);

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}

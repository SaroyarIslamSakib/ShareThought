using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using MapsterMapper;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class AddBlogAreaCommandHandlerTests
{
    private AutoMock _moq;
    private AddBlogAreaCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ISlugService> _slugServiceMock;

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
        _mapperMock = _moq.Mock<IMapper>();
        _slugServiceMock = _moq.Mock<ISlugService>();

        _handler = _moq.Create<AddBlogAreaCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _mapperMock?.Reset();
        _slugServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_AddsBlogAreaSuccessfully()
    {
        // Arrange
        var command = new AddBlogAreaCommand
        {
            Name = "Technology",
            Description = "Tech related blogs"
        };

        var mappedBlogArea = new BlogArea
        {
            Name = command.Name,
            Description = command.Description
        };

        var generatedSlug = "technology";

        _mapperMock
            .Setup(x => x.Map<BlogArea>(command))
            .Returns(mappedBlogArea)
            .Verifiable();

        _slugServiceMock
            .Setup(x => x.GenerateUniqueSlugAsync(command.Name))
            .ReturnsAsync(generatedSlug)
            .Verifiable();

        _blogAreaRepositoryMock
            .Setup(x => x.AddAsync(It.Is<BlogArea>(b =>
                b.Name == command.Name &&
                b.Description == command.Description &&
                b.Slug == generatedSlug &&
                b.Id != Guid.Empty)))
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
            () => result.ShouldNotBeNull(),
            () => result.ShouldBeOfType<BlogArea>(),
            () => result.Name.ShouldBe(command.Name),
            () => result.Description.ShouldBe(command.Description),
            () => result.Slug.ShouldBe(generatedSlug),
            () => result.Id.ShouldNotBe(Guid.Empty),

            () => _mapperMock.Verify(x => x.Map<BlogArea>(command), Times.Once),
            () => _slugServiceMock.Verify(x => x.GenerateUniqueSlugAsync(command.Name), Times.Once),
            () => _blogAreaRepositoryMock.VerifyAll(),
            () => _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once)
        );
    }
}

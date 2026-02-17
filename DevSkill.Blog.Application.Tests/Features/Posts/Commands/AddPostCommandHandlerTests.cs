using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class AddPostCommandHandlerTests
{
    private AutoMock _moq;
    private AddPostCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<ITagRepository> _tagRepositoryMock;
    private Mock<IPostRepository> _postRepositoryMock;

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
        _categoryRepositoryMock = _moq.Mock<ICategoryRepository>();
        _tagRepositoryMock = _moq.Mock<ITagRepository>();
        _postRepositoryMock = _moq.Mock<IPostRepository>();

        _handler = _moq.Create<AddPostCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _categoryRepositoryMock?.Reset();
        _tagRepositoryMock?.Reset();
        _postRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesPostWithCategoriesAndTags()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();

        var blog = new BlogArea
        {
            Id = blogId,
            UserId = userId
        };

        var command = new AddPostCommand
        {
            UserId = userId,
            Title = "Test Post",
            Content = "Test Content",
            CreatedAt = DateTime.UtcNow,
            FeatureImagePath = "image.jpg",
            CategoryNames = new List<string> { "Tech", "Programming" },
            TagNames = new List<string> { "CSharp", "DotNet" }
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog });

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Category)null);

        _tagRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Tag)null);

        _categoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        _tagRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Tag>()))
            .Returns(Task.CompletedTask);

        _postRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Post>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.CategoryRepository)
            .Returns(_categoryRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.TagRepository)
            .Returns(_tagRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        _unitOfWorkMock.Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBe(Guid.Empty);

        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Exactly(2));
        _tagRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tag>()), Times.Exactly(2));
        _postRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Post>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_BlogNotFound_ThrowsException()
    {
        // Arrange
        var command = new AddPostCommand
        {
            UserId = Guid.NewGuid()
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(command.UserId))
            .ReturnsAsync(new List<BlogArea>());

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

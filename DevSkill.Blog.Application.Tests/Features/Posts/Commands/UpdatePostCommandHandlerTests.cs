using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class UpdatePostCommandHandlerTests
{
    private AutoMock _moq;
    private UpdatePostCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IPostRepository> _postRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<ITagRepository> _tagRepositoryMock;
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
        _postRepositoryMock = _moq.Mock<IPostRepository>();
        _categoryRepositoryMock = _moq.Mock<ICategoryRepository>();
        _tagRepositoryMock = _moq.Mock<ITagRepository>();
        _slugServiceMock = _moq.Mock<ISlugService>();

        _handler = _moq.Create<UpdatePostCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _postRepositoryMock?.Reset();
        _categoryRepositoryMock?.Reset();
        _tagRepositoryMock?.Reset();
        _slugServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_UpdatesPostSuccessfully()
    {
        var postId = Guid.NewGuid();

        var post = new Post
        {
            Id = postId,
            Title = "Old Title",
            Slug = "old-title",
            Content = "Old Content",
            FeatureImagePath = "old.jpg",
            PostCategories = new List<Category>(),
            Tags = new List<Tag>()
        };

        var command = new UpdatePostCommand
        {
            PostId = postId,
            Title = "New Title",
            Content = "New Content",
            FeatureImagePath = "new.jpg",
            CategoryNames = new List<string> { "Tech" },
            TagNames = new List<string> { "CSharp" }
        };

        _postRepositoryMock
            .Setup(x => x.GetPostWithCategoriesTagsAsync(postId))
            .ReturnsAsync(post);

        _slugServiceMock
            .Setup(x => x.GenerateUniqueSlugAsync("New Title"))
            .ReturnsAsync("new-title");

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Category)null);

        _categoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        _tagRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Tag)null);

        _tagRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Tag>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.CategoryRepository)
            .Returns(_categoryRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.TagRepository)
            .Returns(_tagRepositoryMock.Object);

        _unitOfWorkMock.Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Title.ShouldBe("New Title");
        result.Content.ShouldBe("New Content");
        result.FeatureImagePath.ShouldBe("new.jpg");
        result.Slug.ShouldBe("new-title");
        result.PostCategories.Count.ShouldBe(1);
        result.Tags.Count.ShouldBe(1);

        _slugServiceMock.Verify(x => x.GenerateUniqueSlugAsync("New Title"), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_PostNotFound_ThrowsException()
    {
        var command = new UpdatePostCommand
        {
            PostId = Guid.NewGuid(),
            Title = "Test"
        };

        _postRepositoryMock
            .Setup(x => x.GetPostWithCategoriesTagsAsync(command.PostId))
            .ReturnsAsync((Post)null);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

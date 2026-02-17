using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Domain.Utilities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class PublishPostCommandHandlerTests
{
    private AutoMock _moq;
    private PublishPostCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
    private Mock<IPostRepository> _postRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<ITagRepository> _tagRepositoryMock;
    private Mock<IServerTime> _serverTimeMock;
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
        _postRepositoryMock = _moq.Mock<IPostRepository>();
        _categoryRepositoryMock = _moq.Mock<ICategoryRepository>();
        _tagRepositoryMock = _moq.Mock<ITagRepository>();
        _serverTimeMock = _moq.Mock<IServerTime>();
        _slugServiceMock = _moq.Mock<ISlugService>();

        _handler = _moq.Create<PublishPostCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _postRepositoryMock?.Reset();
        _categoryRepositoryMock?.Reset();
        _tagRepositoryMock?.Reset();
        _serverTimeMock?.Reset();
        _slugServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_PublishesPostSuccessfully()
    {
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var blog = new BlogArea { Id = blogId, UserId = userId };

        var post = new Post
        {
            Id = postId,
            Title = "Test Post",
            BlogAreaId = blogId,
            IsPublished = false
        };

        var now = DateTime.UtcNow;

        var command = new PublishPostCommand
        {
            UserId = userId,
            PostId = postId,
            FeatureImagePath = "new-image.jpg",
            CategoryNames = new List<string> { "Tech" },
            TagNames = new List<string> { "CSharp" }
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog });

        _postRepositoryMock
            .Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(post);

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

        _slugServiceMock
            .Setup(x => x.GenerateUniqueSlugAsync(post.Title))
            .ReturnsAsync("test-post");

        _serverTimeMock
            .Setup(x => x.DateTime)
            .Returns(now);

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

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

        result.ShouldBe(postId);
        post.IsPublished.ShouldBeTrue();
        post.PublishedAt.ShouldBe(now);
        post.Slug.ShouldBe("test-post");
        post.FeatureImagePath.ShouldBe("new-image.jpg");

        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_BlogNotFound_ThrowsException()
    {
        var command = new PublishPostCommand
        {
            UserId = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            CategoryNames = new List<string>(),
            TagNames = new List<string>()
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(command.UserId))
            .ReturnsAsync(new List<BlogArea>());

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_PostNotFound_ThrowsException()
    {
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { new BlogArea { Id = blogId, UserId = userId } });

        _postRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Post)null);

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        var command = new PublishPostCommand
        {
            UserId = userId,
            PostId = Guid.NewGuid(),
            CategoryNames = new List<string>(),
            TagNames = new List<string>()
        };

        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

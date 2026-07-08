using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Domain.Utilities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class SaveDraftPostCommandHandlerTests
{
    private AutoMock _moq;
    private SaveDraftPostCommandHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
    private Mock<IPostRepository> _postRepositoryMock;
    private Mock<IServerTime> _serverTimeMock;

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
        _serverTimeMock = _moq.Mock<IServerTime>();

        _handler = _moq.Create<SaveDraftPostCommandHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _postRepositoryMock?.Reset();
        _serverTimeMock?.Reset();
    }

    [Test]
    public async Task Handle_NewDraft_CreatesDraftAndReturnsId()
    {
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var blog = new BlogArea { Id = blogId, UserId = userId };

        var command = new SaveDraftPostCommand
        {
            UserId = userId,
            Title = "Draft Title",
            Content = "Draft Content"
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog });

        _serverTimeMock
            .Setup(x => x.DateTime)
            .Returns(now);

        _postRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Post>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        _unitOfWorkMock.Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);

        _postRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Post>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_UpdateExistingDraft_UpdatesAndReturnsId()
    {
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var blog = new BlogArea { Id = blogId, UserId = userId };

        var existingDraft = new Post
        {
            Id = postId,
            BlogAreaId = blogId,
            Title = "Old Title",
            Content = "Old Content",
            IsPublished = false
        };

        var command = new SaveDraftPostCommand
        {
            Id = postId,
            UserId = userId,
            Title = "Updated Title",
            Content = "Updated Content"
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog });

        _postRepositoryMock
            .Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(existingDraft);

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        _unitOfWorkMock.Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(postId);
        existingDraft.Title.ShouldBe("Updated Title");
        existingDraft.Content.ShouldBe("Updated Content");

        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_BlogNotFound_ThrowsException()
    {
        var command = new SaveDraftPostCommand
        {
            UserId = Guid.NewGuid(),
            Title = "Test",
            Content = "Test"
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
    public async Task Handle_InvalidExistingDraft_ThrowsException()
    {
        var userId = Guid.NewGuid();
        var blogId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var blog = new BlogArea { Id = blogId, UserId = userId };

        var publishedPost = new Post
        {
            Id = postId,
            BlogAreaId = blogId,
            IsPublished = true
        };

        var command = new SaveDraftPostCommand
        {
            Id = postId,
            UserId = userId,
            Title = "Updated",
            Content = "Updated"
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<BlogArea> { blog });

        _postRepositoryMock
            .Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(publishedPost);

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

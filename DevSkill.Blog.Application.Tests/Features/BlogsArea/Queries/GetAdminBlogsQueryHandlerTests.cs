using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Infrastructure.Identity;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Linq.Expressions;

namespace DevSkill.Blog.Application.Tests;

public class GetAdminBlogsQueryHandlerTests
{
    private AutoMock _moq;
    private GetAdminBlogsQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogAreaRepositoryMock;
    private Mock<IPostRepository> _postRepositoryMock;
    private Mock<IUserService> _userServiceMock;

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
        _userServiceMock = _moq.Mock<IUserService>();

        _handler = _moq.Create<GetAdminBlogsQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogAreaRepositoryMock?.Reset();
        _postRepositoryMock?.Reset();
        _userServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsMappedBlogDtos()
    {
        // Arrange
        var query = new GetAdminBlogsQuery
        {
            PageIndex = 1,
            PageSize = 10,
            SearchText = "",
            SortOrder = "asc"
        };

        var blogId1 = Guid.NewGuid();
        var blogId2 = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var blogs = new List<BlogArea>
        {
            new BlogArea
            {
                Id = blogId1,
                Name = "Blog 1",
                UserId = userId1,
                CreatedAt = DateTime.UtcNow,
                IsSuspended = false,
                Slug = "blog-1"
            },
            new BlogArea
            {
                Id = blogId2,
                Name = "Blog 2",
                UserId = userId2,
                CreatedAt = DateTime.UtcNow,
                IsSuspended = true,
                Slug = "blog-2"
            }
        };

        _blogAreaRepositoryMock
            .Setup(x => x.GetAdminPagedBlogsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder))
            .ReturnsAsync((blogs, 2, 2));

        _userServiceMock
     .Setup(x => x.GetUserByIdAsync(userId1))
     .ReturnsAsync(new UserListDto
     {
         Id = userId1,
         FullName = "User One",
         Email = "user1@test.com"
     });

        _userServiceMock
            .Setup(x => x.GetUserByIdAsync(userId2))
            .ReturnsAsync(new UserListDto
            {
                Id = userId2,
                FullName = "User Two",
                Email = "user2@test.com"
            });

        _postRepositoryMock
            .Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Post, bool>>>()))
            .ReturnsAsync(5);

        _unitOfWorkMock
            .SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogAreaRepositoryMock.Object);

        _unitOfWorkMock
            .SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.total.ShouldBe(2);
        result.totalDisplay.ShouldBe(2);
        result.Item1.Count.ShouldBe(2);

        var first = result.Item1[0];
        first.Title.ShouldBe("Blog 1");
        first.OwnerEmail.ShouldBe("user1@test.com");
        first.TotalPosts.ShouldBe(5);
        first.BlogSlug.ShouldBe("blog-1");

        _blogAreaRepositoryMock.Verify(x =>
            x.GetAdminPagedBlogsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder), Times.Once);

        _userServiceMock.Verify(x =>
            x.GetUserByIdAsync(It.IsAny<Guid>()), Times.Exactly(2));

        _postRepositoryMock.Verify(x =>
            x.GetCountAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Exactly(2));
    }
}

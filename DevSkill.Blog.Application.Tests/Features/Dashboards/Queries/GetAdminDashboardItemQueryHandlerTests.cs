using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Dashboards.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetAdminDashboardItemQueryHandlerTests
{
    private AutoMock _moq;
    private GetAdminDashboardItemQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<IBlogAreaRepository> _blogRepositoryMock;
    private Mock<IPostRepository> _postRepositoryMock;
    private Mock<IContactMessageRepository> _contactRepositoryMock;
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
        _blogRepositoryMock = _moq.Mock<IBlogAreaRepository>();
        _postRepositoryMock = _moq.Mock<IPostRepository>();
        _contactRepositoryMock = _moq.Mock<IContactMessageRepository>();
        _userServiceMock = _moq.Mock<IUserService>();

        _handler = _moq.Create<GetAdminDashboardItemQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _blogRepositoryMock?.Reset();
        _postRepositoryMock?.Reset();
        _contactRepositoryMock?.Reset();
        _userServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ReturnsAggregatedDashboardCounts()
    {
        // Arrange
        var query = new GetAdminDashboardItemQuery();

        _blogRepositoryMock
            .Setup(x => x.GetCountAsync())
            .ReturnsAsync(5)
            .Verifiable();

        _postRepositoryMock
            .Setup(x => x.GetCountAsync())
            .ReturnsAsync(20)
            .Verifiable();

        _contactRepositoryMock
            .Setup(x => x.GetCountAsync())
            .ReturnsAsync(7)
            .Verifiable();

        _userServiceMock
            .Setup(x => x.GetTotalUserCountAsync())
            .ReturnsAsync(15)
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.BlogAreaRepository)
            .Returns(_blogRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.PostRepository)
            .Returns(_postRepositoryMock.Object)
            .Verifiable();

        _unitOfWorkMock.SetupGet(x => x.ContactMessageRepository)
            .Returns(_contactRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.TotalBlog.ShouldBe(5),
            () => result.TotalPost.ShouldBe(20),
            () => result.TotalMessage.ShouldBe(7),
            () => result.TotalUser.ShouldBe(15),

            () => _blogRepositoryMock.VerifyAll(),
            () => _postRepositoryMock.VerifyAll(),
            () => _contactRepositoryMock.VerifyAll(),
            () => _userServiceMock.VerifyAll()
        );
    }
}

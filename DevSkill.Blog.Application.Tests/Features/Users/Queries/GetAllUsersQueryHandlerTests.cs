using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Users.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain.Dtos;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetAllUsersQueryHandlerTests
{
    private AutoMock _moq;
    private GetAllUsersQueryHandler _handler;

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
        _userServiceMock = _moq.Mock<IUserService>();
        _handler = _moq.Create<GetAllUsersQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _userServiceMock?.Reset();
    }

    [Test]
    public async Task Handle_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserListDto>
        {
            new UserListDto
            {
                Id = Guid.NewGuid(),
                FullName = "User One",
                Email = "user1@test.com"
            },
            new UserListDto
            {
                Id = Guid.NewGuid(),
                FullName = "User Two",
                Email = "user2@test.com"
            }
        };

        _userServiceMock
            .Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(users)
            .Verifiable();

        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.Count.ShouldBe(2),
            () => result[0].FullName.ShouldBe("User One"),
            () => result[1].Email.ShouldBe("user2@test.com"),

            () => _userServiceMock.VerifyAll()
        );
    }
}

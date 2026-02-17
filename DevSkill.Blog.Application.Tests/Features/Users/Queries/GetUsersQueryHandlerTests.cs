using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Users.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Utilities;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetUsersQueryHandlerTests
{
    private AutoMock _moq;
    private GetUsersQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ISqlUtility> _sqlUtilityMock;

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
        _sqlUtilityMock = _moq.Mock<ISqlUtility>();

        _handler = _moq.Create<GetUsersQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _sqlUtilityMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsUsersWithTotals()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            PageIndex = 1,
            PageSize = 10,
            SortOrder = "Name asc",
            Name = "John",
            RegistrationFrom = DateTime.UtcNow.AddMonths(-1),
            RegistrationTo = DateTime.UtcNow
        };

        var users = new List<UserListDto>
        {
            new UserListDto
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "john@test.com"
            }
        };

        var output = (
            result: (IList<UserListDto>)users,
            outValues: new Dictionary<string, object>
            {
                { "Total", 5 },
                { "TotalDisplay", 1 }
            }
        );

        _sqlUtilityMock
            .Setup(x => x.QueryWithStoredProcedureAsync<UserListDto>(
                "GetUsers",
                It.IsAny<Dictionary<string, object?>>(),
                It.IsAny<Dictionary<string, Type>>()))
            .ReturnsAsync(output)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.SqlUtility)
            .Returns(_sqlUtilityMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.Item1.Count.ShouldBe(1),
            () => result.Item1[0].FullName.ShouldBe("John Doe"),
            () => result.total.ShouldBe(5),
            () => result.totalDisplay.ShouldBe(1),

            () => _sqlUtilityMock.VerifyAll()
        );
    }
}

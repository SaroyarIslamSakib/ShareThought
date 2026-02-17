using Autofac.Extras.Moq;
using DevSkill.Blog.Application.Features.Comments.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using Moq;
using Shouldly;

namespace DevSkill.Blog.Application.Tests;

public class GetCommentCountByPostIdQueryHandlerTests
{
    private AutoMock _moq;
    private GetCommentCountByPostIdQueryHandler _handler;

    private Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private Mock<ICommentRepository> _commentRepositoryMock;

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
        _commentRepositoryMock = _moq.Mock<ICommentRepository>();

        _handler = _moq.Create<GetCommentCountByPostIdQueryHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWorkMock?.Reset();
        _commentRepositoryMock?.Reset();
    }

    [Test]
    public async Task Handle_ValidPostId_ReturnsCommentCount()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var expectedCount = 7;

        var query = new GetCommentCountByPostIdQuery
        {
            PostId = postId
        };

        _commentRepositoryMock
            .Setup(x => x.GetCommentCountByPostIdAsync(postId))
            .ReturnsAsync(expectedCount)
            .Verifiable();

        _unitOfWorkMock
            .SetupGet(x => x.CommentRepository)
            .Returns(_commentRepositoryMock.Object)
            .Verifiable();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        this.ShouldSatisfyAllConditions(
            () => result.ShouldBe(expectedCount),
            () => _commentRepositoryMock.VerifyAll()
        );
    }
}

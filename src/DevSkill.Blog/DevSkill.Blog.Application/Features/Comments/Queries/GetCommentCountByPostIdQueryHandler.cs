using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Application.Features.Comments.Queries
{
    public class GetCommentCountByPostIdQueryHandler : IQueryHandler<GetCommentCountByPostIdQuery, int>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetCommentCountByPostIdQueryHandler(
            IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(GetCommentCountByPostIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CommentRepository
            .GetCommentCountByPostIdAsync(query.PostId);
        }
    }
}

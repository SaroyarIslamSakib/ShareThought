using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetDraftPostQueryHandler : IQueryHandler<GetDraftPostQuery, (IList<Post>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetDraftPostQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<Post>, int total, int totalDisplay)> Handle(GetDraftPostQuery query, CancellationToken cancellationToken)
        {
            var blog = (await _unitOfWork.BlogAreaRepository
                 .GetByUserIdAsync(query.UserId))
                 .FirstOrDefault();

            if (blog == null)
                throw new Exception("Blog not found.");

            return await _unitOfWork.PostRepository.GetPagedDraftPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blog.Id);
        }
    }
}

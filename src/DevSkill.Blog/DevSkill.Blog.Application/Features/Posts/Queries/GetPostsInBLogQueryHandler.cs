using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetPostsInBLogQueryHandler : IQueryHandler<GetPostsInBlogQuery, (IList<Domain.Entities.Post>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetPostsInBLogQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<Post>, int total, int totalDisplay)> Handle(GetPostsInBlogQuery query, CancellationToken cancellationToken)
        {

            return await _unitOfWork.PostRepository.GetPagedPostsInBlogBySlugAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                query.CategoryName,
                query.BlogSlug);
        }
    }
}

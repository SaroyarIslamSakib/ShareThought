using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetPostBySlugQueryHandler : IQueryHandler<GetPostBySlugQuery, Post>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetPostBySlugQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> Handle(GetPostBySlugQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PostRepository.GetPostBySlugAsync(query.BlogSlug,query.PostSlug);
        }
    }
}

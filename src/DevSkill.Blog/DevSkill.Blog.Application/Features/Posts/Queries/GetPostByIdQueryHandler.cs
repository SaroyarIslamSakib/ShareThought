using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, Post>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetPostByIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Post> Handle(GetPostByIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PostRepository.GetPostWithCategoriesTagsAsync(query.PostId);
        }
    }
}

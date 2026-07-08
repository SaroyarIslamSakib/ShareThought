using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.BlogsArea.Queries
{
    public class GetBlogAreaBySlugQueryHandler : IQueryHandler<GetBlogAreaBySlugQuery, BlogArea>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetBlogAreaBySlugQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BlogArea> Handle(GetBlogAreaBySlugQuery query, CancellationToken cancellationToken)
        {
            var blog =  await _unitOfWork.BlogAreaRepository.GetBlogBySlug(query.Slug);
            if (blog == null)
                return null;
            return blog;
        }
    }
}

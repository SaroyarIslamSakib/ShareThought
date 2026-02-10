using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetPostsByBlogIdQueryHandler : IQueryHandler<GetPostsByBlogIdQuery, (IList<Post>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetPostsByBlogIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<Post>, int total, int totalDisplay)> Handle(GetPostsByBlogIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PostRepository.GetPublishedPagedPostsAsync(query.PageIndex, query.PageSize, query.SearchText, query.SortOrder, query.BlogId);
        }
    }
}

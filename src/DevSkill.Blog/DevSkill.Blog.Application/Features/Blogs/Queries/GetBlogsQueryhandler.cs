using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Queries
{
    public class GetBlogsQueryhandler : IQueryHandler<GetBlogsQuery, (IList<BlogPost>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetBlogsQueryhandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<BlogPost>, int total, int totalDisplay)> Handle(GetBlogsQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BlogPostRepository.GetPagedBlogsAsync(query.PageIndex, query.PageSize, query.SearchText, query.SortOrder);
        }
    }
}

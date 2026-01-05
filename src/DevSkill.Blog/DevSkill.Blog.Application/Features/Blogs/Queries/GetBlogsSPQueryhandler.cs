using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Queries
{
    public class GetBlogsSPQueryHandler : IQueryHandler<GetBlogsSPQuery, (IList<BlogPostDto>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetBlogsSPQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<BlogPostDto>, int total, int totalDisplay)> Handle(GetBlogsSPQuery query, CancellationToken cancellationToken)
        {
            var procedureName = "GetBlogPosts";

            var output = await _unitOfWork.SqlUtility
                .QueryWithStoredProcedureAsync<BlogPostDto>(procedureName,
                new Dictionary<string, object?>
                {
                    { "PageIndex",  query.PageIndex },
                    { "PageSize", query.PageSize },
                    { "OrderBy", query.SortOrder },
                    { "Title", string.IsNullOrWhiteSpace(query.Title) ? null : query.Title },
                    { "PublishFrom", query.PublishFrom },
                    { "PublishTo", query.PublishTo }
                },
                new Dictionary<string, Type>
                {
                    { "Total", typeof(int) },
                    { "TotalDisplay", typeof(int) }
                });
            return (output.result, (int)output.outValues["Total"], (int)output.outValues["TotalDisplay"]);
        }
    }

}

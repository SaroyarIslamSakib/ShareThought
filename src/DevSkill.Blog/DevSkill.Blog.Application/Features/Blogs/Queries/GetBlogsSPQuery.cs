using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Queries
{
    public class GetBlogsSPQuery : IQuery<(IList<BlogPostDto>, int total, int totalDisplay)>
    {
        public string? Title { get; set; }
        public DateTime? PublishFrom { get; set; }
        public DateTime? PublishTo { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortOrder { get; set; }
    }
}

using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Dashboards.Queries
{
    public class GetBloggerDashboardItemQuery : IQuery<BloggerDashboardDto>
    {
        public Guid BlogId { get; set; }
    }
}

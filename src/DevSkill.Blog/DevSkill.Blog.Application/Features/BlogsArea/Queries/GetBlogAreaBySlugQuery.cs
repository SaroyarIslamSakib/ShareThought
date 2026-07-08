using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Queries
{
    public class GetBlogAreaBySlugQuery : IQuery<BlogArea>
    {
        public Guid UserId { get; set; }
        public string Slug { get; set; }
    }
}

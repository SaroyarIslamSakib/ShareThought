using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Queries
{
    public class BlogPostGetQuery : IQuery<BlogPost>
    {
        public Guid Id { get; set; }
    }
}

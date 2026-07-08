using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetPostBySlugQuery : IQuery<Post>
    {
        public string BlogSlug { get; set; }
        public string PostSlug { get; set; }
    }
}

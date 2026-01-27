using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Queries
{
    public class GetBlogAreaByUserIdQuery : IQuery<BlogArea>
    {
        public Guid UserId { get; set; }
    }
}

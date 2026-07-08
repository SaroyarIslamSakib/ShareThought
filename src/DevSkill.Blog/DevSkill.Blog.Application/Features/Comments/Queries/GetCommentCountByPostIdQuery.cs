using Cortex.Mediator.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Queries
{
    public class GetCommentCountByPostIdQuery : IQuery<int>
    {
        public Guid PostId { get; set; }
    }
}

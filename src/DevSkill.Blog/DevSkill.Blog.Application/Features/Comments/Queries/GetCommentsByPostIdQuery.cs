using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Queries
{
    public class GetCommentsByPostIdQuery : IQuery<IList<CommentDto>>
    {
        public Guid PostId { get; set; }
        public Guid? CurrentUserId { get; set; }
    }
}

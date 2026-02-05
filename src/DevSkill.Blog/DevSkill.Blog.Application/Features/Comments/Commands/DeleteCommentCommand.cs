using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Commands
{
    public class DeleteCommentCommand : ICommand<Guid>
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
    }
}

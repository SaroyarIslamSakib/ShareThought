using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Commands
{
    public class EditCommentCommand : ICommand<CommentDto>
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}

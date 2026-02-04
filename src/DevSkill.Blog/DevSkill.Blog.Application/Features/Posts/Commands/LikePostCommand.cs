using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class LikePostCommand : ICommand<int>
    {
        public Guid PostId { get; set; }
    }
}

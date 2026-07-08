using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class DeletePostCommand:ICommand<Guid>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}

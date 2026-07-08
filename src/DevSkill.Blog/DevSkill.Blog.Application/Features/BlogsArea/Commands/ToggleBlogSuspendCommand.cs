using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Commands
{
    public class ToggleBlogSuspendCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public bool IsSuspended { get; set; }
    }
}

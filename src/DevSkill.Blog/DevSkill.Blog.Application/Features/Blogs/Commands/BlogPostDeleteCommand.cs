using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevSkill.Blog.Application.Features.Blogs.Commands
{
    public class BlogPostDeleteCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}

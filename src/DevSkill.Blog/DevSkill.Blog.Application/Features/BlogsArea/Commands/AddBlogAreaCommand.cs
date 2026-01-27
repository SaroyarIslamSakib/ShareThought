using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Commands
{
    public class AddBlogAreaCommand: ICommand<BlogArea>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class UpdatePostCommand : ICommand<Post>
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FeatureImagePath { get; set; }
        public Guid UserId { get; set; }
        public List<string> CategoryNames { get; set; } = new();
        public List<string> TagNames { get; set; } = new();

    }
}

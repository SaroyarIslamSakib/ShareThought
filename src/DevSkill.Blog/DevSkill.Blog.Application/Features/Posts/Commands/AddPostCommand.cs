using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class AddPostCommand : ICommand<Guid>
    {
        public Guid UserId { get; set; }
        public Guid BlogAreaId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? FeatureImagePath { get; set; }
        public string CategoryNames { get; set; }

    }
}

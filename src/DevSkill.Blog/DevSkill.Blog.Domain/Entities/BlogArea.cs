using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Entities
{
    public class BlogArea : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public bool IsSuspended { get; set; }

        // Navigation Properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();

    }
}

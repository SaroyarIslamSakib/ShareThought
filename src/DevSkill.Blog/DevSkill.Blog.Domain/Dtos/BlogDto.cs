using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Dtos
{
    public class BlogDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
        public int TotalPosts { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSuspended { get; set; }
        public string BlogSlug { get; set; }
    }
}

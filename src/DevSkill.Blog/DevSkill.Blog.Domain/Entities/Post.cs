using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevSkill.Blog.Domain.Entities
{
    public class Post : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }

        public Guid BlogAreaId { get; set; }
        public BlogArea BlogArea { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 
        public string? FeatureImagePath { get; set; }
        public int Likes { get; set; }
        public ICollection<Category> PostCategories { get; set; } = new List<Category>();
    }
}

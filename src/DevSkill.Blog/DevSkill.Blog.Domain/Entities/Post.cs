using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevSkill.Blog.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public Guid BlogAreaId { get; set; }
        public BlogArea BlogArea { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 
    }
}

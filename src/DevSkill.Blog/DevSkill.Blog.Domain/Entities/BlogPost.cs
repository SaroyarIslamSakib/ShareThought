using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Entities
{
    public class BlogPost : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}

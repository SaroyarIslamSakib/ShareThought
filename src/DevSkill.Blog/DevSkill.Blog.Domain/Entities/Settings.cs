using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Entities
{
    public class Settings : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public string? TermsContent { get; set; }
        public string? StorageType { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

using DevSkill.Blog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Entities
{
    public class Report : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }

        public ReportReason? Reason { get; set; }

        public string? CustomReason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

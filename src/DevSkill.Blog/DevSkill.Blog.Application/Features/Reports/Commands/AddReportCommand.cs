using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Reports.Commands
{
    public class AddReportCommand : ICommand<Guid>
    {
        public Guid PostId { get; set; }
        public string? UserId { get; set; }
        public ReportReason? Reason { get; set; }
        public string? CustomReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

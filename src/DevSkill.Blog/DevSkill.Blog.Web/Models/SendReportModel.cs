using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Models
{
    public class SendReportModel
    {
        public Guid PostId { get; set; }
        public string? Reason { get; set; }
        public string? CustomReason { get; set; }
    }
}

using DevSkill.Blog.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class ContactMessageViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public ContactTopicModel Topic { get; set; }

        public string Message { get; set; } = null!;

        public MessageStatusModel Status { get; set; } = MessageStatusModel.Pending;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public string? ReplySubject { get; set; }
        public string? ReplyText { get; set; }
    }
}

using DevSkill.Blog.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Models
{
    public class ContactMessageModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        public ContactTopicModel Topic { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = null!;

        public MessageStatusModel Status { get; set; } = MessageStatusModel.Pending;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

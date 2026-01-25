using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Models
{
    public class CreateBlogAreaModel
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}

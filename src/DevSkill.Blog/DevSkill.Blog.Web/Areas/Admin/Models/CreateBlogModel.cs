using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class CreateBlogModel
    {
        [Required(ErrorMessage ="Please write a Title")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Please write a blog")]
        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

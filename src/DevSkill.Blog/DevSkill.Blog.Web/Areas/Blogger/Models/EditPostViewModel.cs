using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Areas.Blogger.Models
{
    public class EditPostViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile FeatureImage { get; set; }
        public string ExistingFeatureImagePath { get; set; } = null;
        public List<string> CategoryNames { get; set; } = new();
        public List<string> TagNames { get; set; } = new();
    }
}

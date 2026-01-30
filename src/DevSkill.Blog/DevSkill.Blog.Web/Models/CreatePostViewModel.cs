using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Models
{
    public class CreatePostViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile FeatureImage { get; set; }
        public string ExistingFeatureImagePath { get; set; } = null;
    }
}

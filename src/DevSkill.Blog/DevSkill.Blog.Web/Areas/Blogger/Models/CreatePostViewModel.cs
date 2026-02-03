using System.ComponentModel.DataAnnotations;

namespace DevSkill.Blog.Web.Areas.Blogger.Models
{
    public class CreatePostViewModel
    {
        public Guid Id { get; set; }

        // Draft content (set by JS on submit)
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // Publish-time only
        public IFormFile? FeatureImage { get; set; }

        public List<string> CategoryNames { get; set; } = new();
        public List<string> TagNames { get; set; } = new();
        public bool IsPublished { get; set; }
    }
}

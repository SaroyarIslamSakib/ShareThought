namespace DevSkill.Blog.Web.Models
{
    public class PublicPostViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> CategoryNames { get; set; } = new();
        public List<string> TagNames { get; set; } = new();
    }
}

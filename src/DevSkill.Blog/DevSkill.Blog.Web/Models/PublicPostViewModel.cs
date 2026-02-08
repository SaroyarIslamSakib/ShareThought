using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Web.Models
{
    public class PublicPostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> CategoryNames { get; set; } = new();
        public List<string> TagNames { get; set; } = new();
        public int Likes { get; set; }
        public int Comments { get; set; }
        public bool IsSuspended { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}

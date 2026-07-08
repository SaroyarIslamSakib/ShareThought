using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Web.Models
{
    public class BlogPostListModel : DataTables
    {
        public string? CategoryName { get; set; }
        public string? BlogSlug { get; set; }
    }
}

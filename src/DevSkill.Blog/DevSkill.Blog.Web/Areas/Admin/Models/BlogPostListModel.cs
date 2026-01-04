using DevSkill.Blog.Domain;
namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class BlogPostListModel : DataTables
    {
        public BlogPostAdvanceSearchModel SearchItem { get; set; }

    }
}

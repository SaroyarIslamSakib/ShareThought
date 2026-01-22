using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class UserListModel : DataTables
    {
        public UserAdvanceSearchModel SearchItem { get; set; }
    }
}

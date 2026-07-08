namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class UserListByRoleModel
    {
        public string RoleName { get; set; }
        public List<UserViewModel> Users { get; set; } = new();
    }
}

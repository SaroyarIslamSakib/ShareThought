namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class AssignRoleViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public List<RoleItemModel> Roles { get; set; } = new();
    }
}

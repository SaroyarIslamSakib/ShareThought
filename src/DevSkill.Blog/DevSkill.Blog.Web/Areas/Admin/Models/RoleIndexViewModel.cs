using DevSkill.Blog.Infrastructure.Identity;

namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class RoleIndexViewModel
    {
        public IList<ApplicationRole>? Roles { get; set; }
        public CreateRoleModel? CreateRole { get; set; }
    }
}

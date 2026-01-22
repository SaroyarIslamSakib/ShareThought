namespace DevSkill.Blog.Web.Areas.Admin.Models
{
    public class RoleItemModel
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }
}

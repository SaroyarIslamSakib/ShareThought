using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Models
{
    public class AddCommentModel
    {
        public Guid PostId { get; set; }
        [FromForm(Name = "parent")]
        public Guid? ParentId { get; set; }
        public string Content { get; set; }
    }
}

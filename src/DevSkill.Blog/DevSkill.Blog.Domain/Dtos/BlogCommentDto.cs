using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Dtos
{
    public class BlogCommentDto
    {
        public Guid Id { get; set; }
        public Guid? Parent { get; set; }
        public string Content { get; set; }

        public DateTime Created { get; set; }

        public string UserName { get; set; }
        public string PostTitle { get; set; }
        public bool IsApproved { get; set; }
    }
}

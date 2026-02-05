using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Dtos
{
    public class CommentDto
    {
        public Guid id { get; set; }
        public string user_id { get; set; }

        public Guid? parent { get; set; }
        public string content { get; set; }

        public DateTime created { get; set; }

        public string fullname { get; set; }

        public int upvote_count { get; set; }
        public bool user_has_upvoted { get; set; }
        public bool created_by_current_user { get; set; }
        public DateTime? modified { get; set; }

    }
}

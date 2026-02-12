using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Dtos
{
    public class BloggerDashboardDto
    {
        public int TotalPublishedPost { get; set; }
        public int TotalDraftPost { get; set; }
        public int TotalLike { get; set; }
        public int TotalComment { get; set; }

    }
}

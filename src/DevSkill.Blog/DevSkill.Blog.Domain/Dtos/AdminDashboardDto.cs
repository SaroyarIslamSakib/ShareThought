using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Dtos
{
    public class AdminDashboardDto
    {
        public int TotalBlog { get; set; }
        public int TotalPost { get; set; }
        public int TotalUser { get; set; }
        public int TotalMessage { get; set; }

    }
}

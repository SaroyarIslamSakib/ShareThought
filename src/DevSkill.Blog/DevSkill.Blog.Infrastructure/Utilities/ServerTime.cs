using DevSkill.Blog.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Utilities
{
    public class ServerTime : IServerTime
    {
        public DateTime DateTime
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}

using DevSkill.Blog.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Data.Seeds
{
    public class RoleSeed
    {
        public static ApplicationRole[] GetRoles()
        {
            var roles = new ApplicationRole[]
            {
                new ApplicationRole
                {
                    Id = new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "d290f1ee-6c54-4b01-90e6-d701748f0851"
                },
                new ApplicationRole
                {
                    Id = new Guid("e13b3f4a-7c4b-4d2a-9f3b-1c2d3e4f5a6b"),
                    Name = "Blogger",
                    NormalizedName = "BLOGGER",
                    ConcurrencyStamp = "e13b3f4a-7c4b-4d2a-9f3b-1c2d3e4f5a6b"
                }
            };
            return roles;
        }

    }
}

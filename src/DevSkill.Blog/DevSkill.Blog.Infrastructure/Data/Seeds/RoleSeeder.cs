using DevSkill.Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Data.Seeds
{
    public class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = { "Admin", "Blogger" };

            foreach (var roleName in roles)
            {
                var existingRole = await roleManager.FindByNameAsync(roleName);

                if (existingRole == null)
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        Description = $"{roleName} role"
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}

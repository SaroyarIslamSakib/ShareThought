using DevSkill.Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DevSkill.Blog.Infrastructure.Data.Seeds
{
    public class AdminUserSeeder
    {
        public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager)
        {
            string adminEmail = "admin@gmail.com";
            string adminPassword = "Admin@123";

            var existingUser = await userManager.FindByEmailAsync(adminEmail);

            if (existingUser == null)
            {
                var adminUser = new ApplicationUser
                {
                    FirstName = "Sakib",
                    LastName = "Hasan",
                    PhoneNumber = "01710354289",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    if (await userManager.IsInRoleAsync(adminUser, "Admin") == false)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}

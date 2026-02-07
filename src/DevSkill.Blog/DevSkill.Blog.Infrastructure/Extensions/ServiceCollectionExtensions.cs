using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Data;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Infrastructure.Repositories;
using DevSkill.Blog.Infrastructure.Services;
using DevSkill.Blog.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection service)
        {
            service.AddScoped<IBlogAreaRepository, BlogAreaRepository>();
            service.AddScoped<IBlogPostRepository, BlogPostRepository>();
            service.AddScoped<IPostRepository, PostRepository>();
            service.AddScoped<IContactMessageRepository, ContactMessageRepository>();
            service.AddScoped<ICategoryRepository, CategoryRepository>();
            service.AddScoped<ITagRepository, TagRepository>();
            service.AddScoped<IApplicationUnitOfWork,  ApplicationUnitOfWork>();
            service.AddSingleton<IEmailUtility, EmailUtility>();
            service.AddSingleton<IServerTime, ServerTime>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<ICommentRepository, CommentRepository>();
            service.AddScoped<IReportRepository, ReportRepository>();
        }
        //DbContext Configuration
        public static void AddDbContext(this IServiceCollection service,
            string connectionString, Assembly migrationAssembly)
        {
            service.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                (x) => x.MigrationsAssembly(migrationAssembly)));
        }
        //Identity Configuration
        public static void AddIdentity(this IServiceCollection service)
        {
            service
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders();

            //Configure Unauthorize login path
            service.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Home/Index";
                options.AccessDeniedPath = "/Home/Index";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Redirect("/Home/Index?loginRequired=true");
                    return Task.CompletedTask;
                };
            });

            service.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Signin settings modified.
                options.SignIn.RequireConfirmedEmail = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
        }

    }
}

using Cortex.Mediator.DependencyInjection;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Data;
using DevSkill.Blog.Infrastructure.Data.Seeds;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

#region Bootstrap Logger Configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/web-log-.log",rollingInterval:RollingInterval.Day)
    .CreateBootstrapLogger();
#endregion
try
{

    var builder = WebApplication.CreateBuilder(args);

    #region Serilog Configuration
    builder.Host.UseSerilog((context, lc) => lc
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(builder.Configuration)
    );
    #endregion
    
    #region Service Collection based Dependency Injection
    builder.Services.AddDependencyInjection();
    #endregion

    #region Mediator Configuration
    builder.Services.AddCortexMediator(
        builder.Configuration,
        new[] { typeof(Program), typeof(AddPostCommand) },
        options => options.AddDefaultBehaviors());
    #endregion

    #region Mapster Configuration
    builder.Services.AddMapster();
    #endregion

    #region Docker IP Correction
    builder.WebHost.UseUrls("http://*:80");
    #endregion
    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var migrationAssembly = Assembly.GetAssembly(typeof(ApplicationDbContext));


    //Add DbContext
    builder.Services.AddDbContext(connectionString, migrationAssembly!);
    //Add Razor Pages
    builder.Services.AddRazorPages();


    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    #region SMTP Configuration
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
    #endregion
    #region Identity Configuration
    builder.Services.AddIdentity();
    #endregion
    builder.Services.AddControllersWithViews();



    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
    name: "blogPosts",
    pattern: "blog/{blogSlug}/posts/{postSlug}",
    defaults: new { area = "", controller = "Post", action = "PostDetails" });

    app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{blogSlug}/posts",
    defaults: new { area = "", controller = "Blog", action = "Posts" });


    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages()
       .WithStaticAssets();
    //Add RoleSeed 
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await dbInitializer.InitializeAsync();

        await RoleSeeder.SeedAsync(roleManager);
    }
    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Application crashed");
}
finally
{
    Log.CloseAndFlush();
}

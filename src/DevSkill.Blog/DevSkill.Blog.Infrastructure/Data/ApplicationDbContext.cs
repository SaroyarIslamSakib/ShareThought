using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Data.Seeds;
using DevSkill.Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DevSkill.Blog.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
        ApplicationRole,
        Guid,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Enum int to string convertion
            builder.Entity<ContactMessage>()
                   .Property(x => x.Status)
                   .HasConversion<string>();
            builder.Entity<ContactMessage>()
                   .Property(x => x.Topic)
                   .HasConversion<string>();
            builder.Entity<Report>()
                   .Property(r => r.Reason)
                   .HasConversion<string>();
            //Configure BlogArea
            builder.Entity<ApplicationUser>()
                   .HasOne(u => u.BlogArea)
                   .WithOne()
                   .HasForeignKey<BlogArea>(b => b.UserId)
                   .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(builder);

            // BlogArea ↔ Post (One-to-Many)
            builder.Entity<BlogArea>()
                   .HasMany(b => b.Posts)
                   .WithOne(p => p.BlogArea)
                   .HasForeignKey(p => p.BlogAreaId)
                   .OnDelete(DeleteBehavior.Cascade);
                   

            builder.Entity<Post>()
                   .HasIndex(x => x.Slug).IsUnique();
            builder.Entity<BlogArea>()
                   .HasIndex(x => x.Slug).IsUnique();

        }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<BlogArea> BlogAreas { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Settings> Settings { get; set; }

    }
}

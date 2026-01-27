using DevSkill.Blog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace DevSkill.Blog.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public BlogArea? BlogArea { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Dtos
{
    public class UserListDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty; // FirstName + LastName
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public DateTime RegistrationDate { get; set; } 
    }
}

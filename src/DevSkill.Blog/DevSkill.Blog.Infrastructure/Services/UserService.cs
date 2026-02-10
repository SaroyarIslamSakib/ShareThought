using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<IList<UserListDto>> GetAllUsersAsync()
        {
            var users = _context.Users.Where(x => x.Email.Contains("gmail.com")).ToList();
            List<UserListDto> mappedUsers = new List<UserListDto>();

            foreach (var user in users)
            {

                mappedUsers.Add(new UserListDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                    PhoneNumber = user.PhoneNumber,
                });
            }

            return (mappedUsers);
        }
        public async Task<UserListDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    RegistrationDate = u.RegistrationDate,
                    PhoneNumber = u.PhoneNumber
                })
                .FirstOrDefaultAsync();

            return user;
        }
        public async Task<int> GetTotalUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }
    }
}

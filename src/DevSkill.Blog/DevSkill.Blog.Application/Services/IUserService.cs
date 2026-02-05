using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Services
{
    public interface IUserService
    {
        Task<IList<UserListDto>> GetAllUsersAsync();
        Task<UserListDto?> GetUserByIdAsync(Guid userId);
    }
}

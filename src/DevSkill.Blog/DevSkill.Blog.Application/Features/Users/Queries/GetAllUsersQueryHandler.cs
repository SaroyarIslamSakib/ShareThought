using Cortex.Mediator.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Users.Queries
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IList<UserListDto>>
    {
        private readonly IUserService _userService;
        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IList<UserListDto>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetAllUsersAsync();
        }
    }
}

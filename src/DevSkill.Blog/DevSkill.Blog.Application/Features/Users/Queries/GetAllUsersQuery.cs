using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Users.Queries
{
    public class GetAllUsersQuery : IQuery<IList<UserListDto>>
    {
    }
}

using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Users.Queries
{
    public class GetUsersQuery : IQuery<(IList<UserListDto>, int total, int totalDisplay)>
    {
        public string? Name { get; set; }
        public DateTime? RegistrationFrom { get; set; }
        public DateTime? RegistrationTo { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortOrder { get; set; }
    }
}

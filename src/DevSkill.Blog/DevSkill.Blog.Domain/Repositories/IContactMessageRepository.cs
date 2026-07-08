using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface IContactMessageRepository :IRepository<ContactMessage, Guid>
    {
        Task<(IList<ContactMessage>, int total, int totalDisplay)> GetPagedContactMessagesAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder);
    }
}

using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<(IList<Post>, int total, int totalDisplay)> GetPagedPostsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid BlogId);
    }
}

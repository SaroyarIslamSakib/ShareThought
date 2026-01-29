using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Repositories
{
    public class PostRepository : Repository<Post, Guid>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<(IList<Post>, int total, int totalDisplay)> GetPagedPostsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder)
        {
            return await GetDynamicAsync(x => x.Title.Contains(searchText), sortOrder, null, pageIndex, pageSize);
        }
    }
}

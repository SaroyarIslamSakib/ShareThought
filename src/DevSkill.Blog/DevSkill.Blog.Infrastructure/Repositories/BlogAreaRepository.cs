using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Repositories
{
    public class BlogAreaRepository : Repository<BlogArea, Guid>, IBlogAreaRepository
    {
        public BlogAreaRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<(IList<BlogArea>, int total, int totalDisplay)> GetAdminPagedBlogsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder)
        {
            return await GetDynamicAsync(x => x.Name.Contains(searchText)
                                             , sortOrder, null, pageIndex, pageSize);
        }

        public async Task<BlogArea> GetBlogBySlug(string slug)
        {
            var result = await GetAsync(x => x.Slug == slug, null);
            return result.FirstOrDefault();
        }

        public async Task<IList<BlogArea>> GetByUserIdAsync(Guid userId)
        {
            return await GetAsync(
                b => b.UserId == userId,
                q => q.Include(b => b.Posts)
            );
        }

        public async Task<(IList<BlogArea>, int total, int totalDisplay)> GetPagedBlogsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder)
        {
            return await GetDynamicAsync(x => x.Name.Contains(searchText) && x.IsSuspended == false
                                              , sortOrder, null, pageIndex, pageSize);
        }
    }
}

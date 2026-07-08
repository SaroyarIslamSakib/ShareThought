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
    public class BlogPostRepository : Repository<BlogPost, Guid>, IBlogPostRepository
    {
        public BlogPostRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<(IList<BlogPost>, int total, int totalDisplay)> GetPagedBlogsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder)
        {
            return await GetDynamicAsync(x => x.Title.Contains(searchText),sortOrder,null,pageIndex,pageSize);
        }
    }
}

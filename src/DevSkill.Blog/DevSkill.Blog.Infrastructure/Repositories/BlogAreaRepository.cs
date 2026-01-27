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
    public class BlogAreaRepository : Repository<BlogArea, Guid>, IBlogAreaRepository
    {
        public BlogAreaRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<IList<BlogArea>> GetByUserIdAsync(Guid userId)
        {
            return await GetAsync(b => b.UserId == userId,null);
        }
    }
}

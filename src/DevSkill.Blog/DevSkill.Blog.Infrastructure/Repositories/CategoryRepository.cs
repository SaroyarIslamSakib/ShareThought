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
    public class CategoryRepository : Repository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<Category> GetByNameAsync(string name)
        {
            return GetAsync(c => c.Name == name, null).Result.FirstOrDefault();
        }

        public async Task<IList<Category>> SearchByNameAsync(string searchTerm)
        {
            var (data, _, _) = await GetAsync(
                filter: t => t.Name.Contains(searchTerm),
                orderBy: q => q.OrderBy(t => t.Name),
                pageIndex: 1,
                pageSize: 10,
                isTrackingOff: true
            );

            return data;
        }
    }
}

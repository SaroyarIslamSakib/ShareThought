using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<Category> GetByNameAsync(string name);
        Task<IList<Category>> SearchByNameAsync(string searchTerm);
    }
}

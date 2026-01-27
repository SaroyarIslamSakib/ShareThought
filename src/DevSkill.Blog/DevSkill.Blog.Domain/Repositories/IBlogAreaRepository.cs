using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface IBlogAreaRepository : IRepository<BlogArea, Guid>
    {
        Task<IList<BlogArea>> GetByUserIdAsync(Guid userId);    
    }
}

using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface IRepository<TAggregateRoot, TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
        where TKey : IComparable
    {
        void Add(TAggregateRoot entity);
        Task AddAsync(TAggregateRoot entity);
        void Edit(TAggregateRoot entityToUpdate);
        IList<TAggregateRoot> GetAll();
        Task<IList<TAggregateRoot>> GetAllAsync();
        TAggregateRoot GetById(TKey id);
        Task<TAggregateRoot> GetByIdAsync(TKey id);
        int GetCount(Expression<Func<TAggregateRoot, bool>> filter = null);
        Task<int> GetCountAsync(Expression<Func<TAggregateRoot, bool>> filter = null);
        void Remove(Expression<Func<TAggregateRoot, bool>> filter);
        void Remove(TAggregateRoot entityToDelete);
        void Remove(TKey id);
        void Update(TAggregateRoot entity);
        Task RemoveAsync(Expression<Func<TAggregateRoot, bool>> filter);
        Task RemoveAsync(TAggregateRoot entityToDelete);
        Task RemoveAsync(TKey id);

    }
}

using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        public ISqlUtility SqlUtility { get; private set; }
        public UnitOfWork(DbContext dbContext )
        {
            _dbContext = dbContext;
            SqlUtility = new SqlUtility(_dbContext.Database.GetDbConnection());
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}

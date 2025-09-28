using DevSkill.Blog.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain
{
    public interface IApplicationUnitOfWork : IUnitOfWork
    {
        public IBlogPostRepository BlogPostRepository { get; }
    }
}

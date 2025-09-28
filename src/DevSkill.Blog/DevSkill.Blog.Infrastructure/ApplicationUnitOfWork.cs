using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {
        public IBlogPostRepository BlogPostRepository { get; private set; }

        public ApplicationUnitOfWork(ApplicationDbContext context, IBlogPostRepository blogPostRepository)
            : base(context)
        {
            BlogPostRepository = blogPostRepository;
        }
    }

}

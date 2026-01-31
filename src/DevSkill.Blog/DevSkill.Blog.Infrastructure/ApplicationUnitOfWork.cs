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
        public IContactMessageRepository ContactMessageRepository { get; private set; }
        public IBlogAreaRepository BlogAreaRepository { get; private set; }
        public IPostRepository PostRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }


        public ApplicationUnitOfWork(ApplicationDbContext context, IBlogPostRepository blogPostRepository,
            IContactMessageRepository contactMessageRepository, IBlogAreaRepository blogAreaRepository,
            IPostRepository postRepository, ICategoryRepository categoryRepository)
            : base(context)
        {
            BlogPostRepository = blogPostRepository;
            ContactMessageRepository = contactMessageRepository;
            BlogAreaRepository = blogAreaRepository;
            PostRepository = postRepository;
            CategoryRepository = categoryRepository;

        }
    }

}

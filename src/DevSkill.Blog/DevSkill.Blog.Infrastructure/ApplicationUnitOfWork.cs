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
        public ITagRepository TagRepository { get; private set; }
        public ICommentRepository CommentRepository { get; private set; }
        public IReportRepository ReportRepository { get; private set; }
        public ISettingsRepository SettingsRepository { get; private set; }


        public ApplicationUnitOfWork(ApplicationDbContext context, IBlogPostRepository blogPostRepository,
            IContactMessageRepository contactMessageRepository, IBlogAreaRepository blogAreaRepository,
            IPostRepository postRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository,
            ICommentRepository commentRepository, IReportRepository reportRepository, ISettingsRepository settingsRepository)
            : base(context)
        {
            BlogPostRepository = blogPostRepository;
            ContactMessageRepository = contactMessageRepository;
            BlogAreaRepository = blogAreaRepository;
            PostRepository = postRepository;
            CategoryRepository = categoryRepository;
            TagRepository = tagRepository;
            CommentRepository = commentRepository;
            ReportRepository = reportRepository;
            SettingsRepository = settingsRepository;
        }
    }

}

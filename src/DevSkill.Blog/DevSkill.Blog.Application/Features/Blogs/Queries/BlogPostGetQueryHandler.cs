using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Queries
{
    public class BlogPostGetQueryHandler : IQueryHandler<BlogPostGetQuery, BlogPost>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public BlogPostGetQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BlogPost> Handle(BlogPostGetQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BlogPostRepository.GetByIdAsync(query.Id);
        }
    }
}

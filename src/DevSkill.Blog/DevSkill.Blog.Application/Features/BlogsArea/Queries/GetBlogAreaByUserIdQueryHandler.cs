using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Queries
{
    public class GetBlogAreaByUserIdQueryHandler : IQueryHandler<GetBlogAreaByUserIdQuery, BlogArea>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetBlogAreaByUserIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BlogArea> Handle(GetBlogAreaByUserIdQuery query, CancellationToken cancellationToken)
        {
             var blogArea = await _unitOfWork.BlogAreaRepository.GetByUserIdAsync(query.UserId);
            return blogArea.FirstOrDefault();
        }
    }
}

using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Queries
{
    public class GetPublishedPostQueryHandler : IQueryHandler<GetPublishedPostQuery, (IList<Domain.Entities.Post>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetPublishedPostQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<Post>, int total, int totalDisplay)> Handle(GetPublishedPostQuery query, CancellationToken cancellationToken)
        {
            var blog = (await _unitOfWork.BlogAreaRepository
                .GetByUserIdAsync(query.UserId))
                .FirstOrDefault();

            if (blog == null)
                throw new Exception("Blog not found.");

            return await _unitOfWork.PostRepository.GetPublishedPagedPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blog.Id);
        }
    }
}

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
    public class GetPublicPostQueryHandler : IQueryHandler<GetPublicPostQuery, (IList<Domain.Entities.Post>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetPublicPostQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<Post>, int total, int totalDisplay)> Handle(GetPublicPostQuery query, CancellationToken cancellationToken)
        {

            return await _unitOfWork.PostRepository.GetPagedPublicPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                query.CategoryName);
        }
    }
}

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
    public class GetAdminPostQueryHandler : IQueryHandler<GetAdminPostQuery, (IList<Domain.Entities.Post>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetAdminPostQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(IList<Post>, int total, int totalDisplay)> Handle(GetAdminPostQuery query, CancellationToken cancellationToken)
        {

            return await _unitOfWork.PostRepository.GetPagedAdminPostsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder);
        }
    }
}

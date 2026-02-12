using Cortex.Mediator.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Dashboards.Queries
{
    public class GetBloggerDashboardItemQueryHandler : IQueryHandler<GetBloggerDashboardItemQuery, BloggerDashboardDto>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public GetBloggerDashboardItemQueryHandler(IApplicationUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<BloggerDashboardDto> Handle(GetBloggerDashboardItemQuery query, CancellationToken cancellationToken)
        {
            var dashboardItem = new BloggerDashboardDto()
            {
                TotalPublishedPost = await _unitOfWork.PostRepository.GetCountAsync(x => x.BlogAreaId == query.BlogId && x.IsPublished),
                TotalDraftPost = await _unitOfWork.PostRepository.GetCountAsync(x => x.BlogAreaId == query.BlogId && !x.IsPublished),
                TotalLike = await _unitOfWork.PostRepository.TotalLikeCountInBlogAsync(query.BlogId),
                TotalComment = await _unitOfWork.PostRepository.TotalCommentCountInBlogAsync(query.BlogId),
            };
            return dashboardItem;
        }
    }
}

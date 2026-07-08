using Cortex.Mediator.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;

namespace DevSkill.Blog.Application.Features.Dashboards.Queries
{
    public class GetAdminDashboardItemQueryHandler : IQueryHandler<GetAdminDashboardItemQuery, AdminDashboardDto>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public GetAdminDashboardItemQueryHandler(IApplicationUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<AdminDashboardDto> Handle(GetAdminDashboardItemQuery query, CancellationToken cancellationToken)
        {
            var dashboardItem = new AdminDashboardDto()
            {
                TotalBlog = await _unitOfWork.BlogAreaRepository.GetCountAsync(),
                TotalPost = await _unitOfWork.PostRepository.GetCountAsync(),
                TotalMessage = await _unitOfWork.ContactMessageRepository.GetCountAsync(),
                TotalUser = await _userService.GetTotalUserCountAsync(),
            };
            return dashboardItem;
        }
    }
}

using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Dashboards.Queries;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Web.Areas.Admin.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DashboardController(ILogger<DashboardController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var query = new GetAdminDashboardItemQuery();
            var item = await _mediator.SendQueryAsync<GetAdminDashboardItemQuery,AdminDashboardDto>(query);
            var model = new DashboardItemModel()
            {
                TotalBlog = item.TotalBlog,
                TotalMessage = item.TotalMessage,
                TotalPost = item.TotalPost,
                TotalUser = item.TotalUser,
            };
            return View(model);
        }
    }
}

using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Dashboards.Queries;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IMediator _mediator;
        public DashboardController(ILogger<DashboardController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var query = new GetAdminDashboardItemQuery();

                var item = await _mediator
                    .SendQueryAsync<GetAdminDashboardItemQuery, AdminDashboardDto>(query);

                var model = new DashboardItemModel()
                {
                    TotalBlog = item.TotalBlog,
                    TotalMessage = item.TotalMessage,
                    TotalPost = item.TotalPost,
                    TotalUser = item.TotalUser,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading dashboard data.");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load dashboard data",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
    }
}

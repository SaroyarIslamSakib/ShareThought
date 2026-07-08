using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Dashboards.Queries;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Areas.Blogger.Models;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Blog.Web.Areas.Blogger.Controllers
{
    [Area("Blogger"),Authorize]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IServerTime _serverTime;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogController(ILogger<BlogController> logger, IMediator mediator, IMapper mapper, IServerTime serverTime, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _serverTime = serverTime;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var query = new GetBlogAreaByUserIdQuery
                {
                    UserId = user.Id
                };

                var blog = await _mediator
                    .SendQueryAsync<GetBlogAreaByUserIdQuery, BlogArea>(query);
                if(blog is null)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "You haven't any blog. Please Create a Blog",
                        Response = ResponseTypes.danger
                    });
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                var item = await _mediator
                    .SendQueryAsync<GetBloggerDashboardItemQuery, BloggerDashboardDto>
                        (new GetBloggerDashboardItemQuery() { BlogId = blog.Id });

                var model = new BloggerDashboardItemModel()
                {
                    TotalPublishedPost = item.TotalPublishedPost,
                    TotalComment = item.TotalComment,
                    TotalDraftPost = item.TotalDraftPost,
                    TotalLike = item.TotalLike,
                };
                ViewBag.BlogName = blog.Name;
                ViewBag.BlogSlug = blog.Slug;
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

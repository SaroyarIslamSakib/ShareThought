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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogAreaModel model)
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

                var hasBlog = await _mediator
                    .SendQueryAsync<GetBlogAreaByUserIdQuery, BlogArea>(query);

                if (hasBlog != null)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "You already have a blog",
                        Response = ResponseTypes.danger
                    });

                    return RedirectToAction("Index", "Home");
                }

                var command = new AddBlogAreaCommand
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedAt = _serverTime.DateTime,
                    UserId = user.Id,
                    UserName = user.FirstName + " " + user.LastName
                };

                await _mediator
                    .SendCommandAsync<AddBlogAreaCommand, BlogArea>(command);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog created successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violation
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = ex.Message,
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException)
            {
                // DB constraint (Unique / FK)
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "You already have a blog (database constraint).",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                // Unknown error
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Something went wrong. Please try again later.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Home");
            }
        }


    }
}

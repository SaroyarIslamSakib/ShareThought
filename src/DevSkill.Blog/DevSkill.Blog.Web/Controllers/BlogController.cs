using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
namespace DevSkill.Blog.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServerTime _serverTime;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public BlogController(
            ILogger<BlogController> logger,
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IServerTime serverTime,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
            _serverTime = serverTime;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetBlogsJsonData([FromBody] BlogListModel model)
        {
            try
            {
                var query = new GetBlogsQuery();
                query.SearchText = model.Search.Value;
                query.PageSize = model.PageSize;
                query.PageIndex = model.PageIndex;


                var (items, total, totalDisplay) = _mediator.SendQueryAsync<GetBlogsQuery, (IList<BlogDto>, int total, int totalDisplay)>(query).Result;

                var messages = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                        HttpUtility.HtmlEncode(item.Title),
                        HttpUtility.HtmlEncode(item.OwnerName),
                        HttpUtility.HtmlEncode(item.OwnerEmail),
                        item.TotalPosts.ToString(),
                        item.CreatedAt.ToString("dd-MM-yyyy"),
                        item.IsSuspended.ToString(),
                        item.Id.ToString(),
                        item.BlogSlug.ToString()
                            }).ToArray()
                };
                return Json(messages);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching contact messages.");
                return Json(DataTables.EmptyResult);
            }
        }
        public async Task<IActionResult> Posts(string blogSlug)
        {
            var query = new GetBlogAreaBySlugQuery() { Slug = blogSlug };
            var blog = await _mediator.SendQueryAsync<GetBlogAreaBySlugQuery, BlogArea>(query);
            ViewBag.BlogSlug = blogSlug;
            ViewBag.BlogTitle = blog.Name;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetPostsInBlogJsonData([FromBody] BlogPostListModel model)
        {
            try
            {
                var query = new GetPostsInBlogQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    CategoryName = model.CategoryName,
                    BlogSlug = model.BlogSlug,
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetPostsInBlogQuery,
                        (IList<Post>, int, int)>(query);


                return Json(new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = items.Select(p => new[]
                    {
                        HttpUtility.HtmlEncode(
                            string.IsNullOrEmpty(p.FeatureImagePath)
                                ? "/uploads/features/default_feature_img.png"
                                : p.FeatureImagePath),
                        HttpUtility.HtmlEncode(p.Title),
                        HttpUtility.HtmlEncode(p.Content),
                        p.CreatedAt.ToString("dd-MM-yyyy"),
                        p.Likes.ToString(),
                        p.Id.ToString(),
                        p.Comments.Where(x => x.IsApproved && !x.IsDeleted).Count().ToString(),
                        p.BlogArea.UserName.ToString(),
                        p.BlogArea.Slug.ToString(),
                        p.Slug.ToString()
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPostsJsonData");
                return Json(DataTables.EmptyResult);
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

                    return RedirectToAction("Index", "Home", new { area = "" });
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
                await _userManager.AddToRoleAsync(user, "Blogger");
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Index", "Blog", new { area = "Blogger" });
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

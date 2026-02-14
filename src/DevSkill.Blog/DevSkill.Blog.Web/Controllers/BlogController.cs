using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
namespace DevSkill.Blog.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogController(
            ILogger<BlogController> logger,
            IMediator mediator,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
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
                //query.SortOrder = model.FormatSortExpression("Title");
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
        public IActionResult Posts(string blogSlug)
        {
            ViewBag.BlogSlug = blogSlug;
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
    }
}

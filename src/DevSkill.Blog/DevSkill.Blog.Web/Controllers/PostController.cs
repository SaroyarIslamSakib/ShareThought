using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Comments.Queries;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DevSkill.Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(
            ILogger<PostController> logger,
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
        [HttpGet("/blog/{blogSlug}/{postSlug}")]
        public async Task<IActionResult> PostDetails(string blogSlug, string postSlug)
        {
            try
            {
                var query = new GetPostBySlugQuery()
                {
                    BlogSlug = blogSlug,
                    PostSlug = postSlug
                };
                var post = await _mediator.SendQueryAsync<GetPostBySlugQuery, Post>(query);
                IndexViewModel model = new IndexViewModel()
                {
                    PublicPostModel = new PublicPostViewModel()
                    {
                        Title = post.Title,
                        Content = post.Content,
                        CategoryNames = post.PostCategories.Select(pc => pc.Name).ToList(),
                        TagNames = post.Tags.Select(t => t.Name).ToList(),
                        Likes = post.Likes,
                        Id = post.Id,
                        Comments = post.Comments.Count()
                    }

                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Index", "Post");
            }
        }


        /* =========================
          Public Post - DATATABLE
       ==========================*/
        [HttpPost]
        public async Task<JsonResult> GetPublicPostsJsonData([FromBody] PublicPostListModel model)
        {
            try
            {
                var query = new GetPublicPostQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    CategoryName = model.CategoryName
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetPublicPostQuery,
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
        [HttpPost]
        public async Task<IActionResult> LikePost([FromBody] LikePostCommand command)
        {
            try
            {
                var updatedLikes =
                    await _mediator.SendCommandAsync<LikePostCommand, int>(command);

                return Json(new { likes = updatedLikes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Like post failed");
                return BadRequest();
            }
        }
    }
}

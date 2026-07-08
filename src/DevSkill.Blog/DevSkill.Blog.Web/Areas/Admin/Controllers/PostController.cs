using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Application.Features.Comments.Queries;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Blogger.Models;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin,Support")]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public PostController(ILogger<PostController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetPostsJsonData([FromBody] PostListModel model)
        {
            try
            {
                var query = new GetAdminPostQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetAdminPostQuery,
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
                        p.Comments.Count().ToString(),
                        HttpUtility.HtmlEncode(p.BlogArea.Name),
                        p.IsSuspended.ToString(),
                        p.Reports.Count().ToString()
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
        public async Task<IActionResult> ToggleSuspend(Guid id, bool suspend)
        {
            try
            {
                await _mediator.SendCommandAsync<TogglePostSuspendCommand, Guid>
                        (new TogglePostSuspendCommand { Id = id, IsSuspended = suspend });

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while toggling suspend for Post Id: {PostId}", id);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to update suspend status",
                    Response = ResponseTypes.danger
                });

                return StatusCode(500, new { success = false });
            }
        }

        public async Task<IActionResult> PostReview(Guid id)
        {
            try
            {
                var query = new GetPostByIdQuery()
                {
                    PostId = id
                };
                var post = await _mediator.SendQueryAsync<GetPostByIdQuery, Post>(query);
                var commentsCount = await _mediator.SendQueryAsync<GetCommentCountByPostIdQuery, int>(new GetCommentCountByPostIdQuery { PostId = id });
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
                        Comments = commentsCount,
                        Reports = post.Reports,
                        IsSuspended=post.IsSuspended
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
    }
}

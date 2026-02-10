using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Contacts.Queries;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Areas.Blogger.Models;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogController(ILogger<BlogController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Posts(Guid id)
        {
            ViewBag.BlogId = id;
            return View();
        }
        [HttpPost]
        public JsonResult GetBlogsJsonData([FromBody] BlogListModel model)
        {
            try
            {
                var query = new GetBlogsQuery();
                query.SearchText = model.Search.Value;
                query.SortOrder = model.FormatSortExpression("Name","CreatedAt");
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
                        item.Id.ToString()
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
        [HttpPost]
        public async Task<JsonResult> GetPostsByBlogIdJsonData([FromBody] PostListModel model)
        {
            try
            {
                var query = new GetPostsByBlogIdQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    BlogId = model.BlogId

                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetPostsByBlogIdQuery,
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
                await _mediator.SendCommandAsync<ToggleBlogSuspendCommand, Guid>
                    (new ToggleBlogSuspendCommand { Id = id, IsSuspended = suspend });

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while toggling suspend for Blog Id: {BlogId}", id);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to update suspend status",
                    Response = ResponseTypes.danger
                });

                return StatusCode(500, new { success = false });
            }

        }
    }
}

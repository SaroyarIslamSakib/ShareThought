using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Comments.Commands;
using DevSkill.Blog.Application.Features.Comments.Queries;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Blogger.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Blogger.Controllers
{
    [Area("Blogger"), Authorize]
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IServerTime _serverTime;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ILogger<CommentController> logger, IMediator mediator, IMapper mapper, IServerTime serverTime, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _serverTime = serverTime;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> GetBlogCommentsJsonData([FromBody] CommentListModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(DataTables.EmptyResult);

                var query = new GetCommentsByBlogIdQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Content"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    UserId = user.Id
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetCommentsByBlogIdQuery,
                        (IList<BlogCommentDto>, int, int)>(query);

                return Json(new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = items.Select(p => new[]
                    {
                        HttpUtility.HtmlEncode(p.UserName),
                        HttpUtility.HtmlEncode(p.PostTitle),
                        p.Created.ToString("dd-MM-yyyy"),
                        HttpUtility.HtmlEncode(p.Content),
                        p.IsApproved.ToString(),
                        p.Id.ToString(),
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
        public async Task<IActionResult> ApprovedComment(Guid id)
        {
            try
            {
                var command = new MarkCommentAsApprovedCommand()
                {
                    Id = id
                };

                await _mediator.SendCommandAsync<MarkCommentAsApprovedCommand, Guid>(command);

                return Ok();
            }
            catch (Exception)
            {
                _logger.LogError("Failed to approve comment");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveComment(Guid id)
        {
            try
            {
                var command = new DeleteCommentCommand()
                {
                    CommentId = id
                };

                await _mediator.SendCommandAsync<DeleteCommentCommand, Guid>(command);

                return Ok();
            }
            catch (Exception)
            {
                _logger.LogError("Failed to remove comment");
                return BadRequest();
            }
        }
    }
}

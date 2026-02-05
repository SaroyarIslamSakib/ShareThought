using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Comments.Commands;
using DevSkill.Blog.Application.Features.Comments.Queries;
using DevSkill.Blog.Application.Features.Users.Queries;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(
            ILogger<CommentController> logger,
            IMediator mediator,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromForm] AddCommentModel model)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(model.Content))
                return BadRequest("Comment content is required");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var command = new AddCommentCommand
            {
                PostId = model.PostId,
                ParentId = model.ParentId,
                Content = model.Content,
                UserId = user.Id
            };

            var commentDto = await _mediator.SendCommandAsync<AddCommentCommand,CommentDto>(command);

            return Json(commentDto);
        }


        [HttpGet]
        public async Task<IActionResult> GetComments(Guid postId)
        {
            Guid? userId = null;

            if (User.Identity!.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                userId = user?.Id;
            }

            var query = new GetCommentsByPostIdQuery
            {
                PostId = postId,
                CurrentUserId = userId
            };

            var comments = await _mediator.SendQueryAsync<GetCommentsByPostIdQuery, IList<CommentDto>>(query);

            return Json(comments);
        }
    }
}

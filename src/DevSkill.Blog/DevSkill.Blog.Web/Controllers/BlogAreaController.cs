using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Controllers
{
    public class BlogAreaController : Controller
    {
        private readonly ILogger<BlogAreaController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IServerTime _serverTime;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogAreaController(ILogger<BlogAreaController> logger, IMediator mediator, IMapper mapper, IServerTime serverTime, UserManager<ApplicationUser> userManager)
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

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogAreaModel model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var command = new AddBlogAreaCommand()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = _serverTime.DateTime,
                OwnerId = user.Id,
                OwnerName =  $"{user.FirstName} {user.LastName}"
            };
            var result = await _mediator.SendCommandAsync<AddBlogAreaCommand, BlogArea>(command);
            return RedirectToAction("Index");
        }
    }
}

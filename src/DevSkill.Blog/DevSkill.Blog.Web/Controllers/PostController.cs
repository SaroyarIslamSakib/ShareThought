using Cortex.Mediator;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IServerTime _serverTime;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ILogger<PostController> logger, IMediator mediator, IMapper mapper, IServerTime serverTime, UserManager<ApplicationUser> userManager)
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Create(CreatePostViewModel model)
        {
            

            return View(model);
        }
    }
}

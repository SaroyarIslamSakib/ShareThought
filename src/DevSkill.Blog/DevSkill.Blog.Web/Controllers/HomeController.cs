using System.Diagnostics;
using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Blogs.Commands;
using DevSkill.Blog.Application.Features.Blogs.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger,IMediator mediator, IApplicationUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        { 
            return View();
        }

        public async Task<IActionResult> ContactUs()
        {
            return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ContactMessageModel model)
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

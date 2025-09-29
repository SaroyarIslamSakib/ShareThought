using System.Diagnostics;
using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Blogs.Commands;
using DevSkill.Blog.Application.Features.Blogs.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger,IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            //var command = new BlogPostAddCommand()
            //{
            //    Title = "C++",
            //    Body = "C++ is a Programming language",
            //};
            //var post = await _mediator.SendCommandAsync<BlogPostAddCommand, BlogPost>(command);

            //var query = new BlogPostGetQuery() { Id = new Guid("07f5012e-6ffd-cd75-aabe-08ddffa9f2d5") };
            //var post = await _mediator.SendQueryAsync<BlogPostGetQuery, BlogPost>(query);
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

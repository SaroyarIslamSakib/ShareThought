using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Blogs.Commands;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Admin.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateBlogModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<BlogPostAddCommand>(model);
                    var result = await _mediator.SendCommandAsync<BlogPostAddCommand,BlogPost>(command);

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Blog Post Create Successfully",
                        Response = ResponseTypes.success
                    });
                    return RedirectToAction("Create");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("Error", "Failed to create Blog Post");
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Failed to create Blog Post",
                        Response = ResponseTypes.danger
                    });

                }
            }
            return View();
        }
    }
}

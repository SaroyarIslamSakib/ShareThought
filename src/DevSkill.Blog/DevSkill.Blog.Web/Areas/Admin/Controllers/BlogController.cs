using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Blogs.Commands;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Admin.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Application.Features.Blogs.Queries;
using System.Web;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Web.Models;

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
                    return RedirectToAction("Index");
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
        [HttpPost]
        public async Task<JsonResult> GetBlogPostJsonData([FromBody] BlogPostListModel model)
        {
            try
            {
                var query = new GetBlogsSPQuery();
                query.Title = model.SearchItem.Title;
                query.PublishFrom = model.SearchItem.PublishFrom;
                query.PageIndex = model.PageIndex;
                query.PageSize = model.PageSize;
                query.SortOrder = model.FormatSortExpression("Title", "Body");

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetBlogsSPQuery, (IList<BlogPostDto>, int total, int totalDisplay)>(query);

                var blogs = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                            HttpUtility.HtmlEncode(item.Title),
                            HttpUtility.HtmlEncode(item.Body)
                            }).ToArray()
                };
                return Json(blogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get blog post data");
                return Json(DataTables.EmptyResult);
            }
        }
    }

}

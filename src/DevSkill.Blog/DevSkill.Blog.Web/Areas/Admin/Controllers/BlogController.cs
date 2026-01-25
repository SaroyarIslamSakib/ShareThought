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
using Microsoft.AspNetCore.Authorization;

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
                    _logger.LogError(ex, "Error occurred while creating blog post");

                    ModelState.AddModelError("Error", "Failed to create Blog Post");
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Failed to create Blog Post",
                        Response = ResponseTypes.danger
                    });
                }
            }
            return View(model);
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
                query.SortOrder = model.FormatSortExpression("Title", "Body" , "CreatedAt");

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetBlogsSPQuery, (IList<BlogPostDto>, int total, int totalDisplay)>(query);

                var blogs = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                            HttpUtility.HtmlEncode(item.Title),
                            HttpUtility.HtmlEncode(item.Body),
                            HttpUtility.HtmlEncode(item.CreatedAt),
                            item.Id.ToString()

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



        [HttpPost]
        public async Task<JsonResult> GetBlogById(Guid id)
        {
            try
            {
                var blog = await _mediator.SendQueryAsync<BlogPostGetQuery, BlogPost>(
                    new BlogPostGetQuery { Id = id }
                );

                if (blog == null)
                    return Json(null);

                return Json(new
                {
                    title = blog.Title,
                    body = blog.Body,
                    createdAt = blog.CreatedAt.ToString("dd MMM yyyy")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get blog by id: {id}");
                return Json(null);
            }
        }

        public async Task<JsonResult> EditAsync(Guid id)
        {
            try
            {
                var blog = await _mediator.SendQueryAsync<BlogPostGetQuery, BlogPost>(
                    new BlogPostGetQuery { Id = id }
                );

                if (blog == null)
                    return Json(null);

                return Json(new
                {
                    id = blog.Id,
                    title = blog.Title,
                    body = blog.Body
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load blog for edit. Id: {id}");
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditAsync([FromBody] UpdateBlogModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            try
            {
                var command = new BlogPostEditCommand
                {
                    Id = model.Id,
                    Title = model.Title,
                    Body = model.Body
                };

                await _mediator.SendCommandAsync<BlogPostEditCommand, Guid>(command);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog updated successfully",
                    Response = ResponseTypes.success
                });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update blog. Id: {model.Id}");

                return Json(new
                {
                    success = false,
                    message = "Failed to update blog"
                });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteAsync(Guid id)
        {
            try
            {
                var command = new BlogPostDeleteCommand
                {
                    Id = id
                };
                await _mediator.SendCommandAsync<BlogPostDeleteCommand, Guid>(command);
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog deleted successfully",
                    Response = ResponseTypes.success
                });
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete blog post");
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to delete blog post",
                    Response = ResponseTypes.danger
                });
                return Json(new { success = false });
            }
        }
        public IActionResult ResponsePartial()
        {
            return PartialView("_ResponsePartial");
        }

    }

}

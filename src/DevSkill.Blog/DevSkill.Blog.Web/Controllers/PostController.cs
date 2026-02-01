using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Categories.Queries;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Application.Features.Tags.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using System.Web;

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
            return View(new CreatePostViewModel());
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                string featureImagePath = model.ExistingFeatureImagePath;

                if (model.FeatureImage != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(model.FeatureImage.FileName);
                    var path = Path.Combine("wwwroot/uploads/features", fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await model.FeatureImage.CopyToAsync(stream);

                    featureImagePath = "/uploads/features/" + fileName;
                }


                if (model.Id == Guid.Empty)
                {
                    // CREATE
                    await _mediator.SendCommandAsync<AddPostCommand, Guid>(new AddPostCommand
                    {
                        Title = model.Title,
                        Content = model.Content,
                        FeatureImagePath = featureImagePath,
                        UserId = user.Id,
                        CreatedAt = _serverTime.DateTime,
                        CategoryNames = model.CategoryNames,
                        TagNames = model.TagNames,
                    });
                }
                else
                {
                    // UPDATE
                    await _mediator.SendCommandAsync<UpdatePostCommand, Post>(new UpdatePostCommand
                    {
                        PostId = model.Id,
                        Title = model.Title,
                        Content = model.Content,
                        FeatureImagePath = featureImagePath,
                        UserId = user.Id,
                        CategoryNames = model.CategoryNames,
                        TagNames = model.TagNames

                    });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var query = new GetPostByIdQuery
            {
                PostId = id,
                UserId = user.Id
            };

            var post = await _mediator.SendQueryAsync<GetPostByIdQuery, Post>(query);
            if (post == null)
                return NotFound();

            var model = new CreatePostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ExistingFeatureImagePath = post.FeatureImagePath,

                CategoryNames = post.PostCategories
                        .Select(c => c.Name)
                        .ToList(),

                TagNames = post.Tags
                   .Select(t => t.Name)
                   .ToList()
            };

            return View("Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine("wwwroot/uploads/posts", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return Json(new
            {
                url = "/uploads/posts/" + fileName
            });
        }
        [HttpPost]
        public async Task<JsonResult> GetPostsJsonData([FromBody] PostListModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(DataTables.EmptyResult);

                var query = new GetPostsQuery();
                query.SearchText = model.Search.Value;
                query.SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt");
                query.PageSize = model.PageSize;
                query.PageIndex = model.PageIndex;
                query.UserId = user.Id;

                var (items, total, totalDisplay) = _mediator.SendQueryAsync<GetPostsQuery, (IList<Post>, int total, int totalDisplay)>(query).Result;


                var posts = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                       HttpUtility.HtmlEncode(string.IsNullOrEmpty(item.FeatureImagePath)
                                             ? "/uploads/features/default_feature_img.png"
                                                : item.FeatureImagePath
                                        ),
                        HttpUtility.HtmlEncode(item.Title),
                        HttpUtility.HtmlEncode(item.Content),
                        item.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                        item.Likes.ToString(),
                        item.Id.ToString()
                            }).ToArray()
                };
                return Json(posts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPostsJsonData");
                return Json(DataTables.EmptyResult);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            await _mediator.SendCommandAsync<DeletePostCommand, Guid>(
                new DeletePostCommand
                {
                    PostId = id,
                    UserId = user.Id
                });
            TempData.Put("ResponseMessage", new ResponseModel
            {
                Message = "Post Deleted Successfully",
                Response = ResponseTypes.success
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var query = new GetTagsQuery
            {
                SearchTerm = term
            };

            var tags = await _mediator.SendQueryAsync<GetTagsQuery, IList<Tag>>(query);

            return Json(tags.Select(t => new
            {
                id = t.Name,
                text = t.Name
            }));
        }

        [HttpGet]
        public async Task<IActionResult> SearchCategory(string term)
        {
            var query = new GetCategoriesQuery
            {
                SearchTerm = term
            };

            var categories = await _mediator
                .SendQueryAsync<GetCategoriesQuery, IList<Category>>(query);

            return Json(categories.Select(c => new
            {
                id = c.Name,
                text = c.Name
            }));
        }
    }
}

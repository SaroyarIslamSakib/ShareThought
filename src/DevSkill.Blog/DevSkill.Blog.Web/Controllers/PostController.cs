using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Features.Posts.Queries;
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
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                string featureImagePath = null;

                if (model.FeatureImage != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(model.FeatureImage.FileName);
                    var path = Path.Combine("wwwroot/uploads/features", fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await model.FeatureImage.CopyToAsync(stream);

                    featureImagePath = "/uploads/features/" + fileName;
                }


                var command = new AddPostCommand
                {
                    Title = model.Title,
                    Content = model.Content,
                    UserId = user.Id,
                    CreatedAt = _serverTime.DateTime,
                    FeatureImagePath = featureImagePath,

                };
                var id = await _mediator.SendCommandAsync<AddPostCommand, Guid>(command);



                return View(model);
            }
            catch
            {
                return View(model);
            }
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
        public JsonResult GetPostsJsonData([FromBody] PostListModel model)
        {
            try
            {
                var query = new GetPostsQuery();
                query.SearchText = model.Search.Value;
                query.SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt");
                query.PageSize = model.PageSize;
                query.PageIndex = model.PageIndex;

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
    }
}

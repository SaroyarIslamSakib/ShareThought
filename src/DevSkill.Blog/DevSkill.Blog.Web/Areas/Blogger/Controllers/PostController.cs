using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Categories.Queries;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Application.Features.Tags.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Blogger.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Blogger.Controllers
{
    [Area("Blogger"), Authorize]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(
            ILogger<PostController> logger,
            IMediator mediator,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }

        /* =========================
           INDEX
        ==========================*/
        public IActionResult Index()
        {
            return View();
        }

        /* =========================
           CREATE (OPEN EDITOR)
        ==========================*/
        public IActionResult Create()
        {
            return View(new CreatePostViewModel
            {
                IsPublished = false
            });
        }

        /* =========================
           EDIT (DRAFT vs PUBLISHED)
        ==========================*/
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var post = await _mediator.SendQueryAsync<GetPostByIdQuery, Post>(
                new GetPostByIdQuery
                {
                    PostId = id,
                    UserId = user.Id
                });

            if (post == null)
                return NotFound();

            // 🟢 Draft → same editor, auto-save ON
            if (!post.IsPublished)
            {
                return View("Create", new CreatePostViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    IsPublished = false
                });
            }
            else
            {
                var query = new GetPostByIdQuery
                {
                    PostId = id,
                    UserId = user.Id
                };

                var model = new EditPostViewModel
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

                return View("EditPublished", model);
            }
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPublished(EditPostViewModel model)
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
                        TagNames = model.TagNames,
                        IsPublished = true

                    });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        /* =========================
           AUTO SAVE DRAFT (AJAX)
        ==========================*/
        [HttpPost]
        public async Task<IActionResult> AutoSaveDraft([FromBody] DraftSaveModel model)
         {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var postId = await _mediator.SendCommandAsync<
                SaveDraftPostCommand, Guid>(
                new SaveDraftPostCommand
                {
                    Id = model.Id,
                    Title = model.Title ?? "",
                    Content = model.Content ?? "",
                    UserId = user.Id
                });

            return Json(new
            {
                postId,
                savedAt = DateTime.Now.ToString("HH:mm:ss")
            });
        }

        /* =========================
           PUBLISH
        ==========================*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(CreatePostViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            if (model.Id == Guid.Empty)
                return RedirectToAction(nameof(Create));

            string? featureImagePath = null;

            if (model.FeatureImage != null && model.FeatureImage.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.FeatureImage.FileName);
                var uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/features",
                    fileName
                );

                using var stream = new FileStream(uploadPath, FileMode.Create);
                await model.FeatureImage.CopyToAsync(stream);

                featureImagePath = "/uploads/features/" + fileName;
            }

            await _mediator.SendCommandAsync<PublishPostCommand, Guid>(
                new PublishPostCommand
                {
                    PostId = model.Id,
                    UserId = user.Id,
                    CategoryNames = model.CategoryNames,
                    TagNames = model.TagNames,
                    FeatureImagePath = featureImagePath
                });

            TempData.Put("ResponseMessage", new ResponseModel
            {
                Message = "Post published successfully",
                Response = ResponseTypes.success
            });

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           UPLOAD IMAGE (EDITOR)
        ==========================*/
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

        /* =========================
           DATATABLE
        ==========================*/
        [HttpPost]
        public async Task<JsonResult> GetPostsJsonData([FromBody] PostListModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(DataTables.EmptyResult);

                var query = new GetPostsQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    UserId = user.Id
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetPostsQuery,
                        (IList<Post>, int, int)>(query);

                return Json(new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = items.Select(p => new[]
                    {
                        HttpUtility.HtmlEncode(
                            string.IsNullOrEmpty(p.FeatureImagePath)
                                ? "/uploads/features/default_feature_img.png"
                                : p.FeatureImagePath),
                        HttpUtility.HtmlEncode(p.Title),
                        HttpUtility.HtmlEncode(p.Content),
                        p.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                        p.Likes.ToString(),
                        p.Id.ToString()
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPostsJsonData");
                return Json(DataTables.EmptyResult);
            }
        }

        /* =========================
           DELETE
        ==========================*/
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
                Message = "Post deleted successfully",
                Response = ResponseTypes.success
            });

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           TAG / CATEGORY SEARCH
        ==========================*/
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var tags = await _mediator.SendQueryAsync<GetTagsQuery, IList<Tag>>(
                new GetTagsQuery { SearchTerm = term });

            return Json(tags.Select(t => new { id = t.Name, text = t.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> SearchCategory(string term)
        {
            var categories = await _mediator.SendQueryAsync<
                GetCategoriesQuery, IList<Category>>(
                new GetCategoriesQuery { SearchTerm = term });

            return Json(categories.Select(c => new { id = c.Name, text = c.Name }));
        }

        /* =========================
           EDIT PUBLISHED (FUTURE)
        ==========================*/
        public IActionResult EditPublished(Guid id)
        {
            return View("EditPublished"); // future
        }
    }
}

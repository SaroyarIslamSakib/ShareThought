using Cortex.Mediator;
using DevSkill.Blog.Application.Features.BlogsArea.Queries;
using DevSkill.Blog.Application.Features.Categories.Queries;
using DevSkill.Blog.Application.Features.Posts.Commands;
using DevSkill.Blog.Application.Features.Posts.Queries;
using DevSkill.Blog.Application.Features.Tags.Queries;
using DevSkill.Blog.Application.Services;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DevSkill.Blog.Web.Areas.Blogger.Controllers
{
    [Area("Blogger"), Authorize(Roles = "Blogger")]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageChecker _imageChecker;

        public PostController(
            ILogger<PostController> logger,
            IMediator mediator,
            UserManager<ApplicationUser> userManager,IImageChecker imageChecker)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
            _imageChecker = imageChecker;
        }

        /* =========================
           Published Posts List
        ==========================*/
        public async Task<IActionResult> PublishedPostList()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var query = new GetBlogAreaByUserIdQuery
                {
                    UserId = user.Id
                };

                var blog = await _mediator
                    .SendQueryAsync<GetBlogAreaByUserIdQuery, BlogArea>(query);

                ViewBag.BlogName = blog.Name;
                ViewBag.BlogSlug = blog.Slug;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while loading PublishedPostList. User: {UserName}",
                    User?.Identity?.Name);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load published posts.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Blog", new { area = "Blogger" });
            }
        }

        /* =========================
           Draft Posts List
        ==========================*/
        public async Task<IActionResult> DraftPostList()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var query = new GetBlogAreaByUserIdQuery
                {
                    UserId = user.Id
                };

                var blog = await _mediator
                    .SendQueryAsync<GetBlogAreaByUserIdQuery, BlogArea>(query);

                ViewBag.BlogName = blog.Name;
                ViewBag.BlogSlug = blog.Slug;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while loading DraftPostList. User: {UserName}",
                    User?.Identity?.Name);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load draft posts.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Blog", new { area = "Blogger" });
            }
        }
        /* =========================
           CREATE (OPEN EDITOR)
        ==========================*/
        public async Task<IActionResult> Create()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = new GetBlogAreaByUserIdQuery() { UserId = user.Id };
                var blog = await _mediator.SendQueryAsync<GetBlogAreaByUserIdQuery, BlogArea>(query);
                ViewBag.BlogName = blog.Name;
                return View(new CreatePostViewModel
                {
                    IsPublished = false
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(new CreatePostViewModel
                {
                    IsPublished = false
                });
            }

        }

        /* =========================
           EDIT (DRAFT vs PUBLISHED)
        ==========================*/
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var getBlogQury = new GetBlogAreaByUserIdQuery()
                {
                    UserId = user.Id
                };

                var blog = await _mediator
                    .SendQueryAsync<GetBlogAreaByUserIdQuery, BlogArea>(getBlogQury);

                ViewBag.BlogName = blog.Name;

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
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while editing post. PostId: {PostId}, User: {UserName}",
                    id,
                    User?.Identity?.Name);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load post for editing.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Post");
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

                if (model.FeatureImage != null && model.FeatureImage.Length > 0)
                {
                    using var validationStream = model.FeatureImage.OpenReadStream();

                    bool isValid = _imageChecker.IsValidImageFile(validationStream, model.FeatureImage.FileName);

                    if (!isValid)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Invalid image file. Only valid images up to 2MB allowed.",
                            Response = ResponseTypes.danger
                        });

                        return RedirectToAction("PublishedPostList", "Post", new { area = "Blogger" });
                    }
                    var extension = Path.GetExtension(model.FeatureImage.FileName);
                    var fileName = Guid.NewGuid() + extension;
                    var uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/features"
                    );

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var uploadPath = Path.Combine(uploadFolder, fileName);

                    using var stream = new FileStream(uploadPath, FileMode.Create);
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

                return RedirectToAction("PublishedPostList");
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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while auto-saving draft. PostId: {PostId}, User: {UserName}",
                    model?.Id,
                    User?.Identity?.Name);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Auto save failed."
                });
            }
        }

        /* =========================
           PUBLISH
        ==========================*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(CreatePostViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                if (model.Id == Guid.Empty)
                    return RedirectToAction(nameof(Create));

                string? featureImagePath = null;

                if (model.FeatureImage != null && model.FeatureImage.Length > 0)
                {
                    using var validationStream = model.FeatureImage.OpenReadStream();

                    bool isValid = _imageChecker.IsValidImageFile(
                        validationStream,
                        model.FeatureImage.FileName);

                    if (!isValid)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Invalid image file. Only valid images up to 2MB allowed.",
                            Response = ResponseTypes.danger
                        });

                        return RedirectToAction("DraftPostList", "Post",
                            new { area = "Blogger" });
                    }

                    var extension = Path.GetExtension(model.FeatureImage.FileName);
                    var fileName = Guid.NewGuid() + extension;

                    var uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/features"
                    );

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var uploadPath = Path.Combine(uploadFolder, fileName);

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

                return RedirectToAction(nameof(PublishedPostList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while publishing post. PostId: {PostId}, User: {UserName}",
                    model?.Id,
                    User?.Identity?.Name);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to publish post.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction(nameof(DraftPostList));
            }
        }


        /* =========================
           UPLOAD IMAGE (EDITOR)
        ==========================*/
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                using var validationStream = file.OpenReadStream();

                if (!_imageChecker.IsValidImageFile(validationStream, file.FileName))
                    return BadRequest("Invalid image file.");

                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid() + extension;

                var uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/posts"
                );

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return Json(new
                {
                    url = "/uploads/posts/" + fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while uploading image. FileName: {FileName}, User: {UserName}",
                    file?.FileName,
                    User?.Identity?.Name);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Image upload failed."
                });
            }
        }


        /* =========================
           Published Post - DATATABLE
        ==========================*/
        [HttpPost]
        public async Task<JsonResult> GetPublishedPostsJsonData([FromBody] PostListModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(DataTables.EmptyResult);

                var query = new GetPublishedPostQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    UserId = user.Id
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetPublishedPostQuery,
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
                        p.CreatedAt.ToString("dd-MM-yyyy"),
                        p.Likes.ToString(),
                        p.Id.ToString(),
                        p.Comments.Count().ToString(),
                        p.IsSuspended.ToString(),
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
           Draft Post - DATATABLE
        ==========================*/
        [HttpPost]
        public async Task<JsonResult> GetDraftPostsJsonData([FromBody] PostListModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(DataTables.EmptyResult);

                var query = new GetDraftPostQuery
                {
                    SearchText = model.Search.Value,
                    SortOrder = model.FormatSortExpression("Title", "Content", "CreatedAt"),
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex,
                    UserId = user.Id
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<
                        GetDraftPostQuery,
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
            try
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

                return RedirectToAction(nameof(PublishedPostList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while deleting post. PostId: {PostId}, User: {UserName}",
                    id,
                    User?.Identity?.Name);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to delete post.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction(nameof(PublishedPostList));
            }
        }


        /* =========================
           TAG / CATEGORY SEARCH
        ==========================*/
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var tags = await _mediator.SendQueryAsync<GetTagsQuery, IList<Tag>>(
                    new GetTagsQuery { SearchTerm = term });

                return Json(tags.Select(t => new { id = t.Name, text = t.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while searching tags. Term: {SearchTerm}, User: {UserName}",
                    term,
                    User?.Identity?.Name);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Tag search failed."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCategory(string term)
        {
            try
            {
                var categories = await _mediator.SendQueryAsync<
                    GetCategoriesQuery, IList<Category>>(
                    new GetCategoriesQuery { SearchTerm = term });

                return Json(categories.Select(c => new { id = c.Name, text = c.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while searching categories. Term: {SearchTerm}, User: {UserName}",
                    term,
                    User?.Identity?.Name);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Category search failed."
                });
            }
        }

        /* =========================
           EDIT PUBLISHED (FUTURE)
        ==========================*/
        public IActionResult EditPublished(Guid id)
        {
            return View("EditPublished"); 
        }
    }
}

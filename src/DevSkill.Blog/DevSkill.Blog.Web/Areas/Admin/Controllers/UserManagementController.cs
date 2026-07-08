using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Users.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRoleManager _roleManager;
        public UserManagementController(ILogger<UserManagementController> logger, IMediator mediator, IMapper mapper, UserManager<ApplicationUser> userManager, ApplicationRoleManager roleManager)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetUserJsonData([FromBody] UserListModel model)
        {
            try
            {
                var query = new GetUsersQuery
                {
                    Name = model.SearchItem.Name,
                    RegistrationFrom = model.SearchItem.RegistrationFrom,
                    RegistrationTo = model.SearchItem.RegistrationTo,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    SortOrder = model.FormatSortExpression("Name", "Email", "PhoneNumber", "Role", "RegistrationDate")
                };

                var (items, total, totalDisplay) =
                    await _mediator.SendQueryAsync<GetUsersQuery,(IList<UserListDto> items, int total, int totalDisplay)>(query);

                var users = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                        HttpUtility.HtmlEncode(item.FullName),
                        HttpUtility.HtmlEncode(item.Email),
                        HttpUtility.HtmlEncode(item.PhoneNumber ?? ""),
                        HttpUtility.HtmlEncode(item.Role ?? ""),
                        HttpUtility.HtmlEncode(item.RegistrationDate.ToString("dd MMM yyyy")),
                        item.Id.ToString()
                            }).ToArray()
                };

                return Json(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user list data");
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load user data",
                    Response = ResponseTypes.danger
                });
                return Json(DataTables.EmptyResult);
            }
        }

        public async Task<IActionResult> AssignRoleModal(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Invalid user id",
                    Response = ResponseTypes.danger
                });

                return BadRequest();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "User not found",
                        Response = ResponseTypes.danger
                    });

                    return RedirectToAction("Index");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.ToList();

                var model = new AssignRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Roles = roles.Select(r => new RoleItemModel
                    {
                        RoleId = r.Id,
                        RoleName = r.Name!,
                        IsAssigned = userRoles.Contains(r.Name!)
                    }).ToList()
                };

                return PartialView("_AssignRoleModalPartial", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load assign role modal. UserId: {userId}");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load role data",
                    Response = ResponseTypes.danger
                });

                return StatusCode(500);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoles(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Invalid request data",
                    Response = ResponseTypes.danger
                });
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user == null)
                    return RedirectToAction(nameof(Index));

                var currentRoles = await _userManager.GetRolesAsync(user);

                if (model.Roles == null || !model.Roles.Any())
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "No roles selected",
                        Response = ResponseTypes.danger
                    });

                    return RedirectToAction(nameof(Index));
                }

                // Get selected roles from model
                var selectedRoles = model.Roles
                    .Where(r => r.IsAssigned)
                    .Select(r => r.RoleName)
                    .ToList();

                // Remove unselected roles
                var rolesToRemove = currentRoles.Except(selectedRoles);
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                // Add newly selected roles
                var rolesToAdd = selectedRoles.Except(currentRoles);
                await _userManager.AddToRolesAsync(user, rolesToAdd);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role Updated Successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to assign roles. UserId: {model.UserId}");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to update roles",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GetUserForDelete(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Json(null);

                return Json(new
                {
                    userId = user.Id,
                    userName = user.UserName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load user for delete. UserId: {userId}");
                return Json(null);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Invalid user id",
                    Response = ResponseTypes.danger
                });

                return BadRequest();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound();

                // ================= Remove Roles =================
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    var roleResult = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!roleResult.Succeeded)
                        return BadRequest(roleResult.Errors);
                }

                // ================= Remove Claims =================
                var claims = await _userManager.GetClaimsAsync(user);
                if (claims.Any())
                {
                    var claimResult = await _userManager.RemoveClaimsAsync(user, claims);
                    if (!claimResult.Succeeded)
                        return BadRequest(claimResult.Errors);
                }

                // ================= Remove Logins =================
                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var login in logins)
                {
                    await _userManager.RemoveLoginAsync(
                        user,
                        login.LoginProvider,
                        login.ProviderKey);
                }

                // ================= Finally Delete User =================
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "User Deleted Successfully",
                    Response = ResponseTypes.danger
                });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete user. UserId: {userId}");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to delete user",
                    Response = ResponseTypes.danger
                });

                return StatusCode(500);
            }
        }

        public IActionResult ResponsePartial()
        {
            return PartialView("_ResponsePartial");
        }
    }
}

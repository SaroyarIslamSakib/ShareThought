using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Infrastructure.Utilities;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleManagementController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IServerTime _serverTime;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ILogger<RoleManagementController> _logger;

        public RoleManagementController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IEmailUtility emailUtility,
            IServerTime serverTime,
            ApplicationRoleManager roleManager,
            ILogger<RoleManagementController> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _serverTime = serverTime;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new RoleIndexViewModel
                {
                    Roles = await _roleManager.Roles.ToListAsync(),
                    CreateRole = new CreateRoleModel(),
                    EditRole = new EditRoleModel(),
                    DeleteRole = new DeleteRoleModel()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load roles");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load roles",
                    Response = ResponseTypes.danger
                });

                return View(new RoleIndexViewModel
                {
                    Roles = new List<ApplicationRole>(),
                    CreateRole = new CreateRoleModel()
                });
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleIndexViewModel model)
        {
            try
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.CreateRole.RoleName,
                    Description = model.CreateRole.Description,
                    ConcurrencyStamp = IdentityGenerator.NewSequentialGuid().ToString()
                });

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Add Role Successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create role");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to add role",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null) return NotFound();

                var vm = new RoleIndexViewModel
                {
                    EditRole = new EditRoleModel
                    {
                        Id = role.Id,
                        RoleName = role.Name,
                        Description = role.Description
                    }
                };

                return PartialView("_EditRoleModalPartial", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load role for edit. Id: {id}");
                return StatusCode(500);
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleIndexViewModel model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.EditRole.Id.ToString());
                if (role == null) return NotFound();

                role.Name = model.EditRole.RoleName;
                role.Description = model.EditRole.Description;

                await _roleManager.UpdateAsync(role);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role updated successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update role. Id: {model.EditRole.Id}");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to update role",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null) return NotFound();

                var vm = new RoleIndexViewModel
                {
                    DeleteRole = new DeleteRoleModel
                    {
                        Id = role.Id,
                        RoleName = role.Name
                    }
                };

                return PartialView("_DeleteRoleModalPartial", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load role for delete. Id: {id}");
                return StatusCode(500);
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RoleIndexViewModel model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.DeleteRole.Id.ToString());
                if (role == null) return NotFound();

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

                foreach (var user in usersInRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }

                await _roleManager.DeleteAsync(role);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role deleted successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete role. Id: {model.DeleteRole.Id}");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to delete role",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction(nameof(Index));
            }
        }




        public async Task<IActionResult> RoleUsersModal(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                    return NotFound();

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

                var model = new RoleIndexViewModel
                {
                    RoleUser = new UserListByRoleModel
                    {
                        RoleName = role.Name,
                        Users = usersInRole.Select(u => new UserViewModel
                        {
                            Name = $"{u.FirstName} {u.LastName}",
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber
                        }).ToList()
                    }
                };

                return PartialView("_RoleUserModalPartial", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load users for role. RoleId: {roleId}");
                return StatusCode(500);
            }
        }
    }
}

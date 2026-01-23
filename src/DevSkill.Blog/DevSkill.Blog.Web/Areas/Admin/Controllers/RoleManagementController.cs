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
        public RoleManagementController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IEmailUtility emailUtility,
            IServerTime serverTime,
            ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _serverTime = serverTime;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var model = new RoleIndexViewModel
            {
                Roles = await _roleManager.Roles.ToListAsync(),
                CreateRole = new CreateRoleModel()
            };
            return View(model);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleIndexViewModel model)
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

        public async Task<IActionResult> Edit(Guid id)
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleIndexViewModel model)
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

        public async Task<IActionResult> Delete(Guid id)
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
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RoleIndexViewModel model)
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

    }
}

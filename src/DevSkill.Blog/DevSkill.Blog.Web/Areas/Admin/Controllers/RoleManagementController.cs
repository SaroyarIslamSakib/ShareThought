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
    }
}

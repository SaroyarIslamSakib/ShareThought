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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRoleManager _roleManager;
        public UserManagementController(ILogger<BlogController> logger, IMediator mediator, IMapper mapper, UserManager<ApplicationUser> userManager, ApplicationRoleManager roleManager)
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
                return Json(DataTables.EmptyResult);
            }
        }

        public async Task<IActionResult> AssignRoleModal(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoles(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

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


    }
}

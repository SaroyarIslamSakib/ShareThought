using Cortex.Mediator;
using DevSkill.Blog.Application.Features.SystemSettings.Queries;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Controllers
{
    public class LegalController : Controller
    {
        private readonly ILogger<LegalController> _logger;
        private readonly IMediator _mediator;

        public LegalController(
            ILogger<LegalController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<IActionResult> Terms()
        {
            try
            {
                var query = new GetSettingsQuery();
                var settings = await _mediator
                    .SendQueryAsync<GetSettingsQuery, Settings>(query);

                var model = new SettingsModel()
                {
                    TermsContent = settings.TermsContent,
                    StorageType = settings.StorageType,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading Terms page");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to load Terms & Conditions.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index", "Home");
            }
        }
    }
}

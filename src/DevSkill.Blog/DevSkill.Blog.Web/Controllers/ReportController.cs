using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Reports.Commands;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Enums;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevSkill.Blog.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IMediator _mediator;
        private readonly IServerTime _serverTime;

        public ReportController(
            ILogger<ReportController> logger,
            IMediator mediator,IServerTime serverTime)
        {
            _logger = logger;
            _mediator = mediator;
            _serverTime = serverTime;
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReport(IndexViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SendReportModel.Reason) &&
                string.IsNullOrWhiteSpace(model.SendReportModel.CustomReason))
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Report send failed. Please enter a valid reason ",
                    Response = ResponseTypes.danger
                });
                return RedirectToAction("Index", "Post");
            }

            var command = new AddReportCommand
            {
                PostId = model.SendReportModel.PostId,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Reason = model.SendReportModel.Reason != null
                            ? Enum.Parse<ReportReason>(model.SendReportModel.Reason)
                            : null,
                CustomReason = model.SendReportModel.CustomReason,
                CreatedAt = _serverTime.DateTime,
            };

            var id = await _mediator.SendCommandAsync<AddReportCommand, Guid>(command);
            TempData.Put("ResponseMessage", new ResponseModel
            {
                Message = "Report send successfully",
                Response = ResponseTypes.success
            });
            return RedirectToAction("Index","Post");
        }
    }
}

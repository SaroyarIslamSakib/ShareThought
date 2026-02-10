using Cortex.Mediator;
using DevSkill.Blog.Application.Features.SystemSettings.Commands;
using DevSkill.Blog.Application.Features.SystemSettings.Queries;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IServerTime _serverTime;
        public SettingsController(ILogger<SettingsController> logger, IMediator mediator, IMapper mapper,IServerTime serverTime)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _serverTime = serverTime;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TermAndCondition(SettingsModel model)
        {
            try
            {
                var query = new GetSettingsQuery();
                var oldsettings = await _mediator.SendQueryAsync<GetSettingsQuery, Settings>(query);
               
                if(oldsettings is null)
                {
                    var command = new AddSettingsCommand()
                    {
                        TermsContent = model.TermsContent,
                        StorageType = model.StorageType,
                        UpdatedAt = _serverTime.DateTime
                    };
                    await _mediator.SendCommandAsync<AddSettingsCommand, Guid>(command);
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Settings changed successfully",
                        Response = ResponseTypes.success
                    });

                    return RedirectToAction("Index");
                }
                else
                {
                    var command = new UpdateSettingsCommand()
                    {
                        OldSettingsId = oldsettings.Id,
                        TermsContent = model.TermsContent,
                        StorageType = model.StorageType,
                        UpdatedAt = _serverTime.DateTime
                    };
                    await _mediator.SendCommandAsync<UpdateSettingsCommand, Settings>(command);
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Settings changed successfully",
                        Response = ResponseTypes.success

                    });
                    return RedirectToAction("Index");
                }
                
            }
            catch(Exception ex)
            {
                _logger.LogError("Failed to change settings");
                return RedirectToAction("Index");
            }
            
        }
    }
}

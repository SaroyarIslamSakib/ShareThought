using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Blogs.Commands;
using DevSkill.Blog.Application.Features.Blogs.Queries;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DevSkill.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IServerTime _serverTime;

        public HomeController(ILogger<HomeController> logger,IMediator mediator, IApplicationUnitOfWork unitOfWork,IMapper mapper, IServerTime serverTime)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serverTime = serverTime;
        }

        public async Task<IActionResult> Index()
        { 
            return View();
        }

        public async Task<IActionResult> ContactUs()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ContactMessageModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var command = _mapper.Map<ContactMessageAddCommand>(model);
                    command.CreatedAt = _serverTime.DateTime;

                    var result = await _mediator
                        .SendCommandAsync<ContactMessageAddCommand, ContactMessage>(command);

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Message send Successfully",
                        Response = ResponseTypes.success
                    });

                    return RedirectToAction("Index");
                }

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to send message",
                    Response = ResponseTypes.danger
                });

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while submitting ContactUs form");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Something went wrong. Please try again.",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index");
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

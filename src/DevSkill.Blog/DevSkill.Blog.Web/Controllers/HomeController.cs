using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger,IMediator mediator,
            IApplicationUnitOfWork unitOfWork, IMapper mapper, IServerTime serverTime,
            IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serverTime = serverTime;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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
                var token = Request.Form["g-recaptcha-response"];

                if (!await IsReCaptchaValid(token, "contact"))
                {
                    ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
                    return View(model);
                }

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

        private async Task<bool> IsReCaptchaValid(string token, string expectedAction)
        {
            var secretKey = _configuration["GoogleReCaptcha:SecretKey"];

            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
                null);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ReCaptchaResponse>(json);

            return result.Success
                   && result.Score >= 0.5
                   && result.Action == expectedAction;
        }
    }
}

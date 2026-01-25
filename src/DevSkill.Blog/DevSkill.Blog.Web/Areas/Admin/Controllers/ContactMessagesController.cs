using Cortex.Mediator;
using DevSkill.Blog.Application.Features.Contacts.Commands;
using DevSkill.Blog.Application.Features.Contacts.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactMessagesController : Controller
    {
        private readonly ILogger<ContactMessage> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public ContactMessagesController(ILogger<ContactMessage> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMessagesJsonData([FromBody] ContactMessageListModel model)
        {
            try
            {
                var query = new GetContactMessageQuery();
                query.SearchText = model.Search.Value;
                query.SortOrder = model.FormatSortExpression("Name", "Email", "Topic", "Status", "CreatedAt");
                query.PageSize = model.PageSize;
                query.PageIndex = model.PageIndex;


                var (items, total, totalDisplay) = _mediator.SendQueryAsync<GetContactMessageQuery, (IList<ContactMessage>, int total, int totalDisplay)>(query).Result;

                var messages = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                        HttpUtility.HtmlEncode(item.Name),
                        HttpUtility.HtmlEncode(item.Email),
                        item.Topic.ToString(),
                        item.Status.ToString(),
                        item.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                        item.Id.ToString()
                            }).ToArray()
                };
                return Json(messages);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching contact messages.");
                return Json(DataTables.EmptyResult);
            }
        }


        public async Task<IActionResult> ViewMessage(Guid Id)
        {
            try
            {
                var query = new GetContactMessageByIdQuery { Id = Id };
                var message = await _mediator
                    .SendQueryAsync<GetContactMessageByIdQuery, ContactMessage>(query);

                if (message == null)
                {
                    return NotFound();
                }

                var command = new MarkContactMessageAsReadCommand { Id = Id };
                await _mediator
                    .SendCommandAsync<MarkContactMessageAsReadCommand, Guid>(command);

                var model = _mapper.Map<ContactMessageModel>(message);
                return PartialView("_ViewMessageModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to view contact message. Id: {Id}");
                return StatusCode(500);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteContactMessageCommand { Id = id };
                await _mediator.SendCommandAsync<DeleteContactMessageCommand, Guid>(command);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Message deleted successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete contact message. Id: {id}");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to delete message",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                var command = new DeleteAllContactMessageCommand();
                await _mediator.SendCommandAsync<DeleteAllContactMessageCommand, Task>(command);

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "All messages deleted successfully",
                    Response = ResponseTypes.success
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete all contact messages");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Failed to delete all messages",
                    Response = ResponseTypes.danger
                });

                return RedirectToAction("Index");
            }
        }

    }
}

using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class MarkContactMessageAsReadCommandHandler : ICommandHandler<MarkContactMessageAsReadCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        public MarkContactMessageAsReadCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }
        public async Task<Guid> Handle(MarkContactMessageAsReadCommand command, CancellationToken cancellationToken)
        {
            var message = await _applicationUnitOfWork.ContactMessageRepository.GetByIdAsync(command.Id);
            if (!message.IsRead is true)
            {
                message.IsRead = true;
            }
            await _applicationUnitOfWork.ContactMessageRepository.EditAsync(message);
            await _applicationUnitOfWork.SaveAsync();
            return message.Id;

        }
    }
}

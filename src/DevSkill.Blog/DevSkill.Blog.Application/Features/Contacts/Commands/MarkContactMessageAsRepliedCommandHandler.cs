using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class MarkContactMessageAsRepliedCommandHandler : ICommandHandler<MarkContactMessageAsRepliedCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        public MarkContactMessageAsRepliedCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }
        public async Task<Guid> Handle(MarkContactMessageAsRepliedCommand command, CancellationToken cancellationToken)
        {
            var message = await _applicationUnitOfWork.ContactMessageRepository.GetByIdAsync(command.Id);
            if (message.Status != MessageStatus.Replied)
            {
                message.Status = MessageStatus.Replied;
            }
            await _applicationUnitOfWork.ContactMessageRepository.EditAsync(message);
            await _applicationUnitOfWork.SaveAsync();
            return message.Id;

        }
    }
}

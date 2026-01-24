using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class DeleteAllContactMessageCommandHandler : ICommandHandler<DeleteAllContactMessageCommand, Task>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public DeleteAllContactMessageCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Task> Handle(DeleteAllContactMessageCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.ContactMessageRepository.RemoveAllAsync();
            await _unitOfWork.SaveAsync();
            return Task.CompletedTask;
        }
    }
}

using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class DeleteContactMessageCommandHandler : ICommandHandler<DeleteContactMessageCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public DeleteContactMessageCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(DeleteContactMessageCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.ContactMessageRepository.RemoveAsync(command.Id);
            await _unitOfWork.SaveAsync();
            return command.Id;
        }
    }
}

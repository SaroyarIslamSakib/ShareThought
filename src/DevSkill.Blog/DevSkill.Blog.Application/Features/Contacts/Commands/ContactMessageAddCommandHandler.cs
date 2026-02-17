using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using MapsterMapper;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class ContactMessageAddCommandHandler : ICommandHandler<ContactMessageAddCommand, ContactMessage>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ContactMessageAddCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ContactMessage> Handle(ContactMessageAddCommand command, CancellationToken cancellationToken)
        {
            var message = _mapper.Map<ContactMessage>(command);
            message.Id = IdentityGenerator.NewSequentialGuid();
            await _unitOfWork.ContactMessageRepository.AddAsync(message);
            await _unitOfWork.SaveAsync();
            return message;
        }
    }
}

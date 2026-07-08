using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.Contacts.Queries
{

    public class GetContactMessageByIdQueryHandler: IQueryHandler<GetContactMessageByIdQuery, ContactMessage>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        public GetContactMessageByIdQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        async Task<ContactMessage> IQueryHandler<GetContactMessageByIdQuery, ContactMessage>.Handle(GetContactMessageByIdQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.ContactMessageRepository.GetByIdAsync(query.Id);
        }
    }
}

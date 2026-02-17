using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.Contacts.Queries
{
    public class GetContactMessageQueryHandler : IQueryHandler<GetContactMessageQuery, (IList<Domain.Entities.ContactMessage>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        public GetContactMessageQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }
        public async Task<(IList<ContactMessage>, int total, int totalDisplay)> Handle(GetContactMessageQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.ContactMessageRepository.GetPagedContactMessagesAsync(query.PageIndex, query.PageSize, query.SearchText, query.SortOrder);
        }
    }
}

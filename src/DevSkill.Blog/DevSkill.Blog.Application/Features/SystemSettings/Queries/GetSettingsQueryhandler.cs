using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;

namespace DevSkill.Blog.Application.Features.SystemSettings.Queries
{
    public class GetSettingsQueryhandler : IQueryHandler<GetSettingsQuery, Settings>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetSettingsQueryhandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Settings> Handle(GetSettingsQuery query, CancellationToken cancellationToken)
        {
            var settings =  await _unitOfWork.SettingsRepository.GetAllAsync();
            if(settings != null)
            {
                return settings.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}

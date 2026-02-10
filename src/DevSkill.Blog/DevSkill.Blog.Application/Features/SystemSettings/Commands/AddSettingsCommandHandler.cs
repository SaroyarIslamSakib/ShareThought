using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.SystemSettings.Commands
{
    public class AddSettingsCommandHandler : ICommandHandler<AddSettingsCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public AddSettingsCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }
        public async Task<Guid> Handle(AddSettingsCommand command, CancellationToken cancellationToken)
        {
            var settings = new Settings()
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                TermsContent = command.TermsContent,
                StorageType = command.StorageType,
                UpdatedAt = command.UpdatedAt,
            };
            await _unitOfWork.SettingsRepository.AddAsync(settings);
            await _unitOfWork.SaveAsync();
            return settings.Id;
        }
    }
}

using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.SystemSettings.Commands
{
    public class UpdateSettingsCommandHandler : ICommandHandler<UpdateSettingsCommand, Settings>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public UpdateSettingsCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Settings> Handle(UpdateSettingsCommand command, CancellationToken cancellationToken)
        {
            var settings = await _unitOfWork.SettingsRepository.GetByIdAsync(command.OldSettingsId);
            if(settings is not null)
            {
                settings.TermsContent = command.TermsContent;
                settings.StorageType = command.StorageType;
                settings.UpdatedAt = command.UpdatedAt;

                await _unitOfWork.SettingsRepository.EditAsync(settings);
                await _unitOfWork.SaveAsync();
                return settings;
            }
            return settings;

        }
    }
}

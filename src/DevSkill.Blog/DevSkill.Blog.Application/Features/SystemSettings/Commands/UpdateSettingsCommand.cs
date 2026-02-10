using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.SystemSettings.Commands
{
    public class UpdateSettingsCommand : ICommand<Settings>
    {
        public Guid OldSettingsId { get; set; }
        public string? TermsContent { get; set; }
        public string? StorageType { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

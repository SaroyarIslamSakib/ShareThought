using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class MarkContactMessageAsReadCommand: ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}

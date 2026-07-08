using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Contacts.Commands
{
    public class ContactMessageAddCommand : ICommand<ContactMessage>
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public ContactTopic Topic { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public MessageStatus Status { get; set; }
    }
}

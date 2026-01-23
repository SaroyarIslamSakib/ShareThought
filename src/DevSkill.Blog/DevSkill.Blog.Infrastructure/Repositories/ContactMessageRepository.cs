using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Repositories
{
    public class ContactMessageRepository : Repository<ContactMessage, Guid>, IContactMessageRepository
    {
        public ContactMessageRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}

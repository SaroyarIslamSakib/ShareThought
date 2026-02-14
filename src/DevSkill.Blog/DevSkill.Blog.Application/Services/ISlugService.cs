using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Services
{
    public interface ISlugService
    {
        Task<string> GenerateUniqueSlugAsync(string title);
    }
}

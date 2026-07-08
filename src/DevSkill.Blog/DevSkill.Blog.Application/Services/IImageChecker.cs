using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Services
{
    public interface IImageChecker
    {
        bool IsValidImageFile(Stream fileStream, string fileName, long maxSizeInMb = 2);
    }
}

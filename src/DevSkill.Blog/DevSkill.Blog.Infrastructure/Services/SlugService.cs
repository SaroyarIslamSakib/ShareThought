using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Services
{
    public class SlugService : ISlugService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public SlugService(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateUniqueSlugAsync(string title)
        {
            string baseSlug = GenerateSlug(title);
            string slug = baseSlug;
            int count = 1;

            while (await _unitOfWork.PostRepository.ExistsBySlugAsync(slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            return slug;
        }

        private string GenerateSlug(string title)
        {
            string slug = title.ToLowerInvariant();

            // Normalize to composed form (VERY IMPORTANT)
            slug = slug.Normalize(NormalizationForm.FormC);

            // Allow:
            // \p{L}  = Letters
            // \p{M}  = Combining marks (কার, ফলা, রেফ)
            // \p{Nd} = Numbers
            slug = Regex.Replace(slug, @"[^\p{L}\p{M}\p{Nd}\s-]", "");

            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Replace(" ", "-");

            return slug;
        }
    }
}

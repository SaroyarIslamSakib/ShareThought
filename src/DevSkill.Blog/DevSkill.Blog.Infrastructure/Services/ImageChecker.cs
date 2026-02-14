using DevSkill.Blog.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Services
{
    public class ImageChecker : IImageChecker
    {
        public bool IsValidImageFile(Stream fileStream, string fileName, long maxSizeInMb = 2)
        {
            if (fileStream == null || fileStream.Length == 0)
                return false;

            // 1️⃣ Size validation
            if (fileStream.Length > maxSizeInMb * 1024 * 1024)
                return false;

            // 2️⃣ Extension validation
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return false;

            // 3️⃣ Magic number validation
            var signatures = new Dictionary<string, List<byte[]>>
            {
                { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
                { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
                { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
                { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
                { ".webp", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } }
            };

            if (!signatures.ContainsKey(extension))
                return false;

            try
            {
                // Read first 4 bytes
                byte[] headerBytes = new byte[4];
                fileStream.Position = 0;
                fileStream.Read(headerBytes, 0, headerBytes.Length);

                // Reset stream position for further use
                fileStream.Position = 0;

                return signatures[extension]
                    .Any(signature =>
                        headerBytes
                            .Take(signature.Length)
                            .SequenceEqual(signature));
            }
            catch
            {
                return false;
            }
        }
    }
}

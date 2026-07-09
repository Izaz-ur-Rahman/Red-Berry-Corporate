using Microsoft.AspNetCore.Http;

namespace RedBerryCorporate.Helpers
{
    public static class ImageHelper
    {
        public static async Task<string?> UploadBlogImageAsync(IFormFile? file, IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                return null;

            var folder = Path.Combine(env.WebRootPath, "uploads", "blogs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"uploads/blogs/{fileName}";
        }
    }
}
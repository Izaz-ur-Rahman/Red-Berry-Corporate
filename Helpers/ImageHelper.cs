using Microsoft.AspNetCore.Http;

namespace RedBerryCorporate.Helpers
{
    public static class ImageHelper
    {
        public static async Task<string?> UploadBlogImageAsync(IFormFile? file, IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var folder = Path.Combine(env.WebRootPath, "uploads", "blogs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var extension = Path.GetExtension(file.FileName);

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Invalid image format.");

            var fileName = $"{Guid.NewGuid()}{extension}";

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/uploads/blogs/{fileName}";
        }

        public static void DeleteImage(string imagePath, IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            imagePath = imagePath.Replace("/", "\\");

            var fullPath = Path.Combine(
                env.WebRootPath,
                imagePath.TrimStart('\\'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
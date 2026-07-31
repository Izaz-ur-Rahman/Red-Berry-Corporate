//using Microsoft.AspNetCore.Http;

//namespace RedBerryCorporate.Helpers
//{
//    public static class ImageHelper
//    {
//        public static async Task<string?> UploadBlogImageAsync(IFormFile? file, IWebHostEnvironment env)
//        {
//            if (file == null || file.Length == 0)
//                return null;

//            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

//            var folder = Path.Combine(env.WebRootPath, "uploads", "blogs");

//            if (!Directory.Exists(folder))
//                Directory.CreateDirectory(folder);

//            var extension = Path.GetExtension(file.FileName);

//            if (!allowedExtensions.Contains(extension))
//                throw new Exception("Invalid image format.");

//            var fileName = $"{Guid.NewGuid()}{extension}";

//            var path = Path.Combine(folder, fileName);

//            using var stream = new FileStream(path, FileMode.Create);

//            await file.CopyToAsync(stream);

//            return $"/uploads/blogs/{fileName}";
//        }

//        public static void DeleteImage(string imagePath, IWebHostEnvironment env)
//        {
//            if (string.IsNullOrWhiteSpace(imagePath))
//                return;

//            imagePath = imagePath.Replace("/", "\\");

//            var fullPath = Path.Combine(
//                env.WebRootPath,
//                imagePath.TrimStart('\\'));

//            if (File.Exists(fullPath))
//            {
//                File.Delete(fullPath);
//            }
//        }
//    }
//}

using Microsoft.AspNetCore.Http;

namespace RedBerryCorporate.Helpers
{
    public static class ImageHelper
    {
        private static readonly Dictionary<string, string[]> AllowedImageTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                {
                    ".jpg",
                    new[] { "image/jpeg" }
                },
                {
                    ".jpeg",
                    new[] { "image/jpeg" }
                },
                {
                    ".png",
                    new[] { "image/png" }
                },
                {
                    ".webp",
                    new[] { "image/webp" }
                },
                {
                    ".gif",
                    new[] { "image/gif" }
                },
                {
                    ".bmp",
                    new[] { "image/bmp", "image/x-ms-bmp" }
                },
                {
                    ".tif",
                    new[] { "image/tiff" }
                },
                {
                    ".tiff",
                    new[] { "image/tiff" }
                },
                {
                    ".avif",
                    new[] { "image/avif" }
                },
                {
                    ".svg",
                    new[] { "image/svg+xml" }
                }
            };

        public static async Task<string?> UploadBlogImageAsync(
            IFormFile? file,
            IWebHostEnvironment env)
        {
            // No file
            if (file == null || file.Length == 0)
                return null;

            // Maximum file size = 10 MB
            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new Exception(
                    "Image size cannot exceed 10 MB.");
            }

            // Get extension
            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            // Validate extension
            if (!AllowedImageTypes.ContainsKey(extension))
            {
                throw new Exception(
                    "Invalid image format. " +
                    "Allowed formats: JPG, JPEG, PNG, WEBP, GIF, BMP, TIFF, AVIF and SVG.");
            }

            // Validate MIME type
            var allowedMimeTypes =
                AllowedImageTypes[extension];

            if (!allowedMimeTypes.Contains(
                    file.ContentType,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid image content type.");
            }

            // Validate WebRootPath
            if (string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                throw new Exception(
                    "Web root path is not configured.");
            }

            // Upload folder
            var folder =
                Path.Combine(
                    env.WebRootPath,
                    "uploads",
                    "blogs");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Generate unique file name
            var fileName =
                $"{Guid.NewGuid():N}{extension}";

            // Physical file path
            var filePath =
                Path.Combine(
                    folder,
                    fileName);

            // Save file
            await using var stream =
                new FileStream(
                    filePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);

            await file.CopyToAsync(stream);

            // Return public URL
            return $"/uploads/blogs/{fileName}";
        }

        public static void DeleteImage(
            string? imagePath,
            IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            var relativePath =
                imagePath
                    .Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString())
                    .TrimStart(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);

            var fullPath =
                Path.Combine(
                    env.WebRootPath,
                    relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
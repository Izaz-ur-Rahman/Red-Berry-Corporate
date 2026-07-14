using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace RedBerryCorporate.Helpers
{
    public class FileHelper
    {
        private readonly IWebHostEnvironment _environment;

        public FileHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Image not found.");

            // Ensure folder exists
            var folderPath = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public void DeleteImage(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fullPath = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                folderName,
                fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
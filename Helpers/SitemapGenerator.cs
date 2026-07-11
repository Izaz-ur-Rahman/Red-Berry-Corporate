using System.Text;
using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Interfaces.Sitemap;

namespace RedBerryCorporate.Helpers
{
    public class SitemapGenerator : ISitemapGenerator
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public SitemapGenerator(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task GenerateAsync()
        {
            var baseUrl = _configuration["SiteSettings:BaseUrl"];

            var blogs = await _context.Blogs
                .Where(x =>
                    x.IsActive &&
                    x.Status == BlogStatus.Published)
                .OrderByDescending(x => x.PublishingDate)
                .ToListAsync();

            var builder = new StringBuilder();

            builder.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");

            builder.AppendLine(
                @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

            // Home Page

            builder.AppendLine("<url>");
            builder.AppendLine($"<loc>{baseUrl}</loc>");
            builder.AppendLine($"<lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            builder.AppendLine("<changefreq>weekly</changefreq>");
            builder.AppendLine("<priority>1.0</priority>");
            builder.AppendLine("</url>");

            foreach (var blog in blogs)
            {
                builder.AppendLine("<url>");

                builder.AppendLine(
                    $"<loc>{baseUrl}/blog/{blog.Slug}</loc>");

                builder.AppendLine(
                    $"<lastmod>{(blog.UpdateDate ?? blog.EntryDate):yyyy-MM-dd}</lastmod>");

                builder.AppendLine("<changefreq>monthly</changefreq>");

                builder.AppendLine("<priority>0.9</priority>");

                builder.AppendLine("</url>");
            }

            builder.AppendLine("</urlset>");

            var wwwroot = Path.Combine(_environment.ContentRootPath, "wwwroot");

            if (!Directory.Exists(wwwroot))
                Directory.CreateDirectory(wwwroot);

            var path = Path.Combine(wwwroot, "sitemap.xml");

            await File.WriteAllTextAsync(path, builder.ToString());
        }
    }
}
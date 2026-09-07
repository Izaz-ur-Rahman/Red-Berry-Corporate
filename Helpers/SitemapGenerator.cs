//using System.Text;
//using Microsoft.EntityFrameworkCore;
//using RedBerryCorporate.Data;
//using RedBerryCorporate.Enums;
//using RedBerryCorporate.Interfaces.Sitemap;

//namespace RedBerryCorporate.Helpers
//{
//    public class SitemapGenerator : ISitemapGenerator
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _environment;
//        private readonly IConfiguration _configuration;

//        public SitemapGenerator(
//            ApplicationDbContext context,
//            IWebHostEnvironment environment,
//            IConfiguration configuration)
//        {
//            _context = context;
//            _environment = environment;
//            _configuration = configuration;
//        }

//        public async Task GenerateAsync()
//        {
//            var baseUrl = _configuration["SiteSettings:BaseUrl"];

//            var blogs = await _context.Blogs
//                .Where(x =>
//                    x.IsActive &&
//                    x.Status == BlogStatus.Published)
//                .OrderByDescending(x => x.PublishingDate)
//                .ToListAsync();

//            var builder = new StringBuilder();

//            builder.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");

//            builder.AppendLine(
//                @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

//            // Home Page

//            builder.AppendLine("<url>");
//            builder.AppendLine($"<loc>{baseUrl}</loc>");
//            builder.AppendLine($"<lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
//            builder.AppendLine("<changefreq>weekly</changefreq>");
//            builder.AppendLine("<priority>1.0</priority>");
//            builder.AppendLine("</url>");

//            foreach (var blog in blogs)
//            {
//                builder.AppendLine("<url>");

//                builder.AppendLine(
//                    $"<loc>{baseUrl}/blog/{blog.Slug}</loc>");

//                //builder.AppendLine(
//                //    $"<lastmod>{(blog.UpdateDate ?? blog.EntryDate):yyyy-MM-dd}</lastmod>");

//                builder.AppendLine("<changefreq>monthly</changefreq>");

//                builder.AppendLine("<priority>0.9</priority>");

//                builder.AppendLine("</url>");
//            }

//            builder.AppendLine("</urlset>");

//            var wwwroot = Path.Combine(_environment.ContentRootPath, "wwwroot");

//            if (!Directory.Exists(wwwroot))
//                Directory.CreateDirectory(wwwroot);

//            var path = Path.Combine(wwwroot, "sitemap.xml");

//            await File.WriteAllTextAsync(path, builder.ToString());
//        }
//    }
//}

using System.Text;
using System.Xml.Linq;
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

        public async Task<string> GenerateXmlAsync()
        {
            var baseUrl =
                _configuration["SiteSettings:BaseUrl"]
                ?? "https://redberry.ae";

            baseUrl = baseUrl.TrimEnd('/');

            var today = DateTime.UtcNow.Date;

            //---------------------------------------------
            // Published Blogs
            //---------------------------------------------

            var blogs = await _context.Blogs
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.Status == BlogStatus.Published)
                .Select(x => new
                {
                    x.Slug,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.PublishedAt,
                    x.PublishingDate,
                    CategoryId = x.CategoryId,
                    CategorySlug = x.CategoryNavigation.Slug,
                    CategoryIsActive = x.CategoryNavigation.IsActive
                })
                .OrderByDescending(x => x.PublishingDate)
                .ToListAsync();

            //---------------------------------------------
            // Active Categories Having Published Blogs
            //---------------------------------------------

            var resourceCategories = blogs
                .Where(x => x.CategoryIsActive)
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.CategorySlug
                })
                .Select(g => g.Key.CategorySlug)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            //---------------------------------------------
            // XML URL Collection
            //---------------------------------------------

            XNamespace sitemapNamespace =
                "http://www.sitemaps.org/schemas/sitemap/0.9";

            var urls = new List<XElement>();

            //---------------------------------------------
            // STATIC WEBSITE ROUTES
            //---------------------------------------------

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/",
                today,
                "weekly",
                "1.0");

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/ambitions",
                today,
                "monthly",
                "0.9");

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/infrastructure",
                today,
                "monthly",
                "0.9");

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/blueprint",
                today,
                "monthly",
                "0.8");

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/blueprint-tool",
                today,
                "monthly",
                "0.8");

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/resources-hub",
                today,
                "weekly",
                "0.9");

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/about",
                today,
                "monthly",
                "0.8");

            //---------------------------------------------
            // AMBITIONS
            //---------------------------------------------

            var ambitionSlugs = new[]
            {
                "launch-a-business",
                "expand-into-the-gcc",
                "grow-and-protect-wealth",
                "create-family-security",
                "increase-global-freedom",
                "build-a-hospitality-venture"
            };

            foreach (var slug in ambitionSlugs)
            {
                AddUrl(
                    urls,
                    sitemapNamespace,
                    baseUrl,
                    $"/ambitions/{slug}",
                    today,
                    "monthly",
                    "0.8");
            }

            //---------------------------------------------
            // INFRASTRUCTURE
            //---------------------------------------------

            var infrastructureSlugs = new[]
            {
                "foundation-build",
                "financial-infrastructure",
                "wealth-structure-design",
                "identity-foundation",
                "venture-architecture",
                "sovereign-freedom",
                "legacy-life-architecture"
            };

            foreach (var slug in infrastructureSlugs)
            {
                AddUrl(
                    urls,
                    sitemapNamespace,
                    baseUrl,
                    $"/infrastructure/{slug}",
                    today,
                    "monthly",
                    "0.8");
            }

            //---------------------------------------------
            // BLUEPRINT
            //---------------------------------------------

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/blueprint/ambition-infrastructure-blueprint",
                today,
                "monthly",
                "0.8");

            //---------------------------------------------
            // ABOUT
            //---------------------------------------------

            var aboutSlugs = new[]
            {
                "philosophy",
                "method",
                "leadership",
                "why-red-berry-exists",
                "partner-network",
                "contact"
            };

            foreach (var slug in aboutSlugs)
            {
                AddUrl(
                    urls,
                    sitemapNamespace,
                    baseUrl,
                    $"/about/{slug}",
                    today,
                    "monthly",
                    "0.7");
            }

            //---------------------------------------------
            // RESOURCES HUB LIBRARY
            //---------------------------------------------

            AddUrl(
                urls,
                sitemapNamespace,
                baseUrl,
                "/resources-hub/ambition-library",
                today,
                "weekly",
                "0.9");

            //---------------------------------------------
            // DYNAMIC RESOURCE CATEGORIES
            //---------------------------------------------

            foreach (var categorySlug in resourceCategories)
            {
                AddUrl(
                    urls,
                    sitemapNamespace,
                    baseUrl,
                    $"/resources-hub/{categorySlug}",
                    today,
                    "weekly",
                    "0.8");
            }

            //---------------------------------------------
            // DYNAMIC BLOG PAGES
            //---------------------------------------------

            foreach (var blog in blogs)
            {
                var lastModified =
                    blog.UpdatedAt
                    ?? blog.PublishedAt
                    ?? blog.PublishingDate
                    ?? blog.CreatedAt;

                AddUrl(
                    urls,
                    sitemapNamespace,
                    baseUrl,
                    $"/blog/{blog.Slug}",
                    lastModified.Date,
                    "monthly",
                    "0.8");
            }

            //---------------------------------------------
            // FINAL XML
            //---------------------------------------------

            var document =
                new XDocument(
                    new XDeclaration(
                        "1.0",
                        "UTF-8",
                        null),
                    new XElement(
                        sitemapNamespace + "urlset",
                        urls));

            return document.ToString(
                SaveOptions.DisableFormatting);
        }

        //---------------------------------------------
        // Generate physical sitemap.xml
        //---------------------------------------------

        public async Task GenerateAsync()
        {
            var xml = await GenerateXmlAsync();

            var wwwroot =
                Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");

            if (!Directory.Exists(wwwroot))
            {
                Directory.CreateDirectory(wwwroot);
            }

            var path =
                Path.Combine(
                    wwwroot,
                    "sitemap.xml");

            await File.WriteAllTextAsync(
                path,
                xml,
                Encoding.UTF8);
        }

        //---------------------------------------------
        // Add URL helper
        //---------------------------------------------

        private static void AddUrl(
            List<XElement> urls,
            XNamespace sitemapNamespace,
            string baseUrl,
            string path,
            DateTime lastModified,
            string changeFrequency,
            string priority)
        {
            var url =
                new XElement(
                    sitemapNamespace + "url",
                    new XElement(
                        sitemapNamespace + "loc",
                        $"{baseUrl}{path}"),
                    new XElement(
                        sitemapNamespace + "lastmod",
                        lastModified.ToString("yyyy-MM-dd")),
                    new XElement(
                        sitemapNamespace + "changefreq",
                        changeFrequency),
                    new XElement(
                        sitemapNamespace + "priority",
                        priority));

            urls.Add(url);
        }
    }
}
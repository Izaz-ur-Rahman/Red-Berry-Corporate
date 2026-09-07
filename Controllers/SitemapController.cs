using Microsoft.AspNetCore.Mvc;
using RedBerryCorporate.Interfaces.Sitemap;

namespace RedBerryCorporate.Controllers
{
    [ApiController]
    public class SitemapController : ControllerBase
    {
        private readonly ISitemapGenerator _sitemapGenerator;

        public SitemapController(
            ISitemapGenerator sitemapGenerator)
        {
            _sitemapGenerator = sitemapGenerator;
        }

        [HttpGet("/sitemap.xml")]
        [Produces("application/xml")]
        public async Task<IActionResult> GetSitemap()
        {
            var xml =
                await _sitemapGenerator.GenerateXmlAsync();

            return Content(
                xml,
                "application/xml",
                System.Text.Encoding.UTF8);
        }
    }
}
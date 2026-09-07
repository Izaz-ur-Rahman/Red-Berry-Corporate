namespace RedBerryCorporate.Interfaces.Sitemap
{
    public interface ISitemapGenerator
    {
        Task<string> GenerateXmlAsync();
        Task GenerateAsync();
    }
}
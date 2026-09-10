namespace RedBerryCorporate.Interfaces.Sitemap
{
    /// <summary>
    /// Delivers a generated sitemap.xml to the public website's own
    /// hosting, so it is served directly from the website domain
    /// instead of depending on the API domain at request time.
    /// </summary>
    public interface ISitemapPublisher
    {
        /// <summary>
        /// Publishes the given sitemap XML content to the website.
        /// Implementations should be best-effort: a failure here must
        /// never throw back into the caller (blog/category save
        /// flows), since publishing the sitemap is a side effect, not
        /// part of the core save operation.
        /// </summary>
        Task PublishAsync(string xml);
    }
}

namespace RedBerryCorporate.Configuration
{
    /// <summary>
    /// Connection details used to publish the generated sitemap.xml
    /// directly onto the redberry.ae website's own hosting (via FTP),
    /// so it can be served from https://redberry.ae/sitemap.xml as a
    /// real static file instead of depending on the API domain at
    /// request time.
    /// </summary>
    public class WebsiteDeploySettings
    {
        /// <summary>
        /// FTP host/IP for the website's hosting account
        /// (e.g. "173.225.111.217" or "ftp.redberry.ae").
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// FTP port. 21 is the standard FTP control port.
        /// </summary>
        public int Port { get; set; } = 21;

        /// <summary>
        /// The website's system/FTP username (e.g. "redberry.ae").
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The website's system/FTP password. Do not commit a real
        /// value here — set it via an environment variable
        /// (WebsiteDeploy__Password) or another secret store instead.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Remote directory (relative to the FTP login's root) where
        /// the sitemap should be placed. On this Plesk/IIS hosting the
        /// FTP root is already the site's home, and the public files
        /// live under "httpdocs".
        /// </summary>
        public string RemoteDirectory { get; set; } = "httpdocs";

        /// <summary>
        /// Final public file name.
        /// </summary>
        public string FileName { get; set; } = "sitemap.xml";

        /// <summary>
        /// Whether to use FTPS (FTP over explicit TLS) instead of
        /// plain FTP. Strongly recommended when the hosting account
        /// supports it, since plain FTP sends credentials in clear
        /// text.
        /// </summary>
        public bool EnableSsl { get; set; } = false;

        /// <summary>
        /// When false (default), publishing is skipped entirely and
        /// only logged — useful for local/dev environments that have
        /// no website FTP credentials configured.
        /// </summary>
        public bool Enabled { get; set; } = false;
    }
}

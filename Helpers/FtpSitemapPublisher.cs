using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedBerryCorporate.Configuration;
using RedBerryCorporate.Interfaces.Sitemap;

namespace RedBerryCorporate.Helpers
{
    /// <summary>
    /// Publishes sitemap.xml to the website's hosting over FTP/FTPS,
    /// using the .NET built-in FtpWebRequest client (no extra NuGet
    /// dependency). Uploads to a temporary file name first and then
    /// renames it into place, so a crawler can never read a
    /// half-written file mid-upload.
    ///
    /// FtpWebRequest is marked obsolete for new code in modern .NET
    /// (there is no first-party replacement client), so the obsolete
    /// warning is intentionally suppressed here.
    /// </summary>
#pragma warning disable SYSLIB0014 // 'WebRequest.Create(string)' is obsolete
    public class FtpSitemapPublisher : ISitemapPublisher
    {
        private readonly WebsiteDeploySettings _settings;
        private readonly ILogger<FtpSitemapPublisher> _logger;

        public FtpSitemapPublisher(
            IOptions<WebsiteDeploySettings> settings,
            ILogger<FtpSitemapPublisher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task PublishAsync(string xml)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation(
                    "WebsiteDeploy is disabled; skipping sitemap publish to the website.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_settings.Host) ||
                string.IsNullOrWhiteSpace(_settings.Username))
            {
                _logger.LogWarning(
                    "WebsiteDeploy Host/Username is not configured; skipping sitemap publish to the website.");
                return;
            }

            var tempFileName =
                $"{_settings.FileName}.tmp-{Guid.NewGuid():N}";

            try
            {
                await UploadAsync(tempFileName, xml);

                await RenameAsync(tempFileName, _settings.FileName);

                _logger.LogInformation(
                    "Sitemap published to website: {Host}/{Dir}/{File}",
                    _settings.Host,
                    _settings.RemoteDirectory,
                    _settings.FileName);
            }
            catch (Exception ex)
            {
                // Best-effort: a failed sitemap push must never break
                // the blog/category action that triggered it.
                _logger.LogError(
                    ex,
                    "Failed to publish sitemap.xml to the website via FTP.");

                // Attempt to clean up the temp file if the rename
                // itself is what failed, so it doesn't linger.
                await TryDeleteAsync(tempFileName);
            }
        }

        private async Task UploadAsync(string remoteFileName, string content)
        {
            var uri = BuildUri(remoteFileName);

            var request = CreateRequest(uri, WebRequestMethods.Ftp.UploadFile);

            var bytes = Encoding.UTF8.GetBytes(content);
            request.ContentLength = bytes.Length;

            using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(bytes);
            }

            using var response = (FtpWebResponse)await request.GetResponseAsync();
        }

        private async Task RenameAsync(string fromFileName, string toFileName)
        {
            var uri = BuildUri(fromFileName);

            var request = CreateRequest(uri, WebRequestMethods.Ftp.Rename);
            request.RenameTo = toFileName;

            using var response = (FtpWebResponse)await request.GetResponseAsync();
        }

        private async Task TryDeleteAsync(string remoteFileName)
        {
            try
            {
                var uri = BuildUri(remoteFileName);

                var request = CreateRequest(uri, WebRequestMethods.Ftp.DeleteFile);

                using var response = (FtpWebResponse)await request.GetResponseAsync();
            }
            catch
            {
                // Ignore — this is only a best-effort cleanup of a
                // stray temp file.
            }
        }

        private FtpWebRequest CreateRequest(Uri uri, string method)
        {
            var request = (FtpWebRequest)WebRequest.Create(uri);

            request.Method = method;
            request.Credentials =
                new NetworkCredential(_settings.Username, _settings.Password);
            request.EnableSsl = _settings.EnableSsl;
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            return request;
        }

        private Uri BuildUri(string fileName)
        {
            var dir = (_settings.RemoteDirectory ?? string.Empty).Trim('/');

            var path = string.IsNullOrEmpty(dir)
                ? fileName
                : $"{dir}/{fileName}";

            return new Uri($"ftp://{_settings.Host}:{_settings.Port}/{path}");
        }
    }
#pragma warning restore SYSLIB0014
}

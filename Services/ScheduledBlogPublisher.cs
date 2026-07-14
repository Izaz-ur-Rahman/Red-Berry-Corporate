using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Interfaces.Sitemap;

namespace RedBerryCorporate.Services
{
    public class ScheduledBlogPublisher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ScheduledBlogPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope =
                    _serviceProvider.CreateScope();

                var repository =
                    scope.ServiceProvider
                        .GetRequiredService<IBlogRepository>();

                var sitemap =
                    scope.ServiceProvider
                        .GetRequiredService<ISitemapGenerator>();

                var blogs =
                    await repository.GetScheduledBlogsAsync();

                bool sitemapNeedsRefresh = false;

                foreach (var blog in blogs)
                {
                    if (blog.PublishingDate.HasValue &&
                        blog.PublishingDate <= DateTime.UtcNow)
                    {
                        blog.Status = BlogStatus.Published;

                        blog.PublishedAt = DateTime.UtcNow;

                        // System User
                        blog.PublishedByUserId = 0;

                        blog.UpdatedAt = DateTime.UtcNow;

                        await repository.PublishScheduledBlogAsync(blog);

                        sitemapNeedsRefresh = true;
                    }
                }

                if (sitemapNeedsRefresh)
                {
                    await sitemap.GenerateAsync();
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
        }
    }
}
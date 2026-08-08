using Microsoft.AspNetCore.Hosting;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Blog.Cards;
using RedBerryCorporate.DTOs.Blog.FAQ;
using RedBerryCorporate.DTOs.Blog.Viewer;
using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Interfaces.BlogCategory;
using RedBerryCorporate.Interfaces.Notification;
using RedBerryCorporate.Interfaces.Sitemap;
using RedBerryCorporate.Models;
using System.Reflection.Metadata;
using System.Text.Json;
namespace RedBerryCorporate.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ISitemapGenerator _sitemap;
        private readonly INotificationService _notificationService;
        private readonly IBlogCategoryRepository _categoryRepository;

        public BlogService(
            IBlogRepository repository,
            IWebHostEnvironment environment,ISitemapGenerator sitemap, INotificationService notificationService, IBlogCategoryRepository categoryRepository)
        {
            _repository = repository;
            _environment = environment;
            _sitemap = sitemap;
            _notificationService = notificationService;
            _categoryRepository = categoryRepository;
        }


public async Task<BlogResponseDto> AddAsync(
    CreateBlogDto dto,
    int currentUserId)
        {
            var slug =
                string.IsNullOrWhiteSpace(dto.Slug)
                    ? SlugHelper.Generate(dto.Title)
                    : SlugHelper.Generate(dto.Slug);

            if (await _repository.SlugExistsAsync(slug))
                throw new Exception("Slug already exists.");

            //-----------------------------------
            // Category Validation
            //-----------------------------------

            var category =
                await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if (category == null || !category.IsActive)
            {
                throw new Exception(
                    "Selected blog category is invalid.");
            }

            //-----------------------------------
            // Upload Image
            //-----------------------------------

            var image =
                await ImageHelper.UploadBlogImageAsync(
                    dto.CoverImage,
                    _environment);

            //-----------------------------------
            // Create Blog
            //-----------------------------------
            var faqList = string.IsNullOrWhiteSpace(dto.FAQs)
    ? new List<BlogFaqInputDto>()
    : JsonSerializer.Deserialize<List<BlogFaqInputDto>>(dto.FAQs)
        ?? new List<BlogFaqInputDto>();
            var blog = new Blog
            {
                Title = dto.Title,
                Slug = slug,

                CategoryId = dto.CategoryId,

                MetaDescription = dto.MetaDescription,
                ShortDescription = dto.ShortDescription,
                BlogDetails = dto.BlogDetails,
                Tags = dto.Tags,
                CoverImage = image,

                ReadTime =
                    CalculateReadTime(dto.BlogDetails),

                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,

                IsActive = true,
                IsDeleted = false,
                FAQs = faqList
    .Select((faq, index) => new BlogFaq
    {
            Question = faq.Question,
            Answer = faq.Answer,
            SortOrder = faq.SortOrder > 0
                ? faq.SortOrder
                : index + 1,
            IsActive = faq.IsActive
        })
        .ToList()
            };

            //-----------------------------------
            // Workflow
            //-----------------------------------

            if (dto.PublishingDate.HasValue)
            {
                if (dto.PublishingDate <= DateTime.UtcNow)
                {
                    blog.Status = BlogStatus.Published;

                    blog.PublishingDate = DateTime.UtcNow;
                    blog.PublishedAt = DateTime.UtcNow;
                    blog.PublishedByUserId = currentUserId;
                }
                else
                {
                    blog.Status = BlogStatus.Scheduled;

                    blog.PublishingDate = dto.PublishingDate;
                }
            }
            else
            {
                blog.Status = BlogStatus.Draft;
            }

            //-----------------------------------
            // Save Blog
            //-----------------------------------
          
            blog = await _repository.AddAsync(blog);

            blog = await _repository.GetByIdAsync(blog.Id);

            if (blog == null)
                throw new Exception(
                    "Blog could not be retrieved after creation.");

            //-----------------------------------
            // Sitemap
            //-----------------------------------

            if (blog.Status == BlogStatus.Published)
                await _sitemap.GenerateAsync();

            //-----------------------------------
            // Notification
            //-----------------------------------

            await _notificationService.CreateAsync(
                title: "New Blog Created",
                message: $"Blog '{blog.Title}' was created successfully.",
                type: NotificationType.Success,
                action: NotificationAction.Created,
                module: NotificationModule.Blog,
                entityId: blog.Id,
                currentUserId: currentUserId);

            return MapToDto(blog);
        }




        public async Task<BlogResponseDto?> UpdateAsync(
            UpdateBlogDto dto,
            int currentUserId)
        {
            var blog = await _repository.GetByIdAsync(dto.Id);

            if (blog == null)
                return null;

            //-----------------------------------
            // Category Validation
            //-----------------------------------

            var category =
                await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if (category == null || !category.IsActive)
            {
                throw new Exception(
                    "Selected blog category is invalid.");
            }

            //-----------------------------------
            // Update Blog Information
            //-----------------------------------

            blog.Title = dto.Title;

            blog.CategoryId = dto.CategoryId;

            blog.MetaDescription = dto.MetaDescription;
            blog.ShortDescription = dto.ShortDescription;
            blog.BlogDetails = dto.BlogDetails;
            blog.Tags = dto.Tags;

            blog.ReadTime =
                CalculateReadTime(dto.BlogDetails);

            //-----------------------------------
            // Prepare FAQs
            //-----------------------------------

            var faqs = dto.FAQs
                .Select((faq, index) => new BlogFaq
                {
                    BlogId = blog.Id,
                    Question = faq.Question,
                    Answer = faq.Answer,
                    SortOrder = faq.SortOrder > 0
                        ? faq.SortOrder
                        : index + 1,
                    IsActive = faq.IsActive
                })
                .ToList();

            //-----------------------------------
            // Slug
            //-----------------------------------

            blog.Slug =
                string.IsNullOrWhiteSpace(dto.Slug)
                    ? SlugHelper.Generate(dto.Title)
                    : SlugHelper.Generate(dto.Slug);

            if (await _repository.SlugExistsAsync(
                blog.Slug,
                blog.Id))
            {
                throw new Exception("Slug already exists.");
            }

            //-----------------------------------
            // Cover Image
            //-----------------------------------

            if (dto.CoverImage != null)
            {
                ImageHelper.DeleteImage(
                    blog.CoverImage,
                    _environment);

                blog.CoverImage =
                    await ImageHelper.UploadBlogImageAsync(
                        dto.CoverImage,
                        _environment);
            }

            //-----------------------------------
            // Audit Information
            //-----------------------------------

            blog.UpdatedAt = DateTime.UtcNow;
            blog.UpdatedByUserId = currentUserId;

            //-----------------------------------
            // Schedule Logic
            //-----------------------------------

            if (dto.PublishingDate.HasValue)
            {
                if (dto.PublishingDate <= DateTime.UtcNow)
                {
                    blog.Status = BlogStatus.Published;

                    blog.PublishingDate = DateTime.UtcNow;
                    blog.PublishedAt = DateTime.UtcNow;
                    blog.PublishedByUserId = currentUserId;
                }
                else
                {
                    blog.Status = BlogStatus.Scheduled;

                    blog.PublishingDate = dto.PublishingDate;
                }
            }

            //-----------------------------------
            // Save Blog
            //-----------------------------------

            blog = await _repository.UpdateAsync(blog);

            //-----------------------------------
            // Replace FAQs
            //-----------------------------------

            await _repository.ReplaceFaqsAsync(
                blog.Id,
                faqs);

            //-----------------------------------
            // Get Updated Blog
            //-----------------------------------

            blog = await _repository.GetByIdAsync(blog.Id);

            if (blog == null)
                return null;

            //-----------------------------------
            // Sitemap
            //-----------------------------------

            if (blog.Status == BlogStatus.Published)
                await _sitemap.GenerateAsync();

            //-----------------------------------
            // Notification
            //-----------------------------------

            await _notificationService.CreateAsync(
                title: "Blog Updated",
                message: $"Blog '{blog.Title}' was updated successfully.",
                type: NotificationType.Info,
                action: NotificationAction.Updated,
                module: NotificationModule.Blog,
                entityId: blog.Id,
                currentUserId: currentUserId);

            return MapToDto(blog);
        }



        public async Task<bool> DeleteAsync(
    int id,
    int currentUserId)
        {
            // Get blog first
            var blog = await _repository.GetByIdForUpdateAsync(id);
            if (blog == null)
                return false;

            var result = await _repository.DeleteAsync(
                id,
                currentUserId);

            if (result)
            {
                await _sitemap.GenerateAsync();

                await _notificationService.CreateAsync(
                    title: "Blog Deleted",
                    message: $"Blog '{blog.Title}' was deleted successfully.",
                    type: NotificationType.Warning,
                    action: NotificationAction.Deleted,
                    module: NotificationModule.Blog,
                    entityId: blog.Id,
                    currentUserId: currentUserId);
            }

            return result;
        }
     

        public async Task<bool> PublishAsync(
    int id,
    int currentUserId)
        {
            var blog = await _repository.GetByIdForUpdateAsync(id);
            if (blog == null)
                return false;

            var result =
                await _repository.PublishAsync(
                    id,
                    currentUserId);

            if (result)
            {
                await _sitemap.GenerateAsync();

                await _notificationService.CreateAsync(
                    title: "Blog Published",
                    message: $"Blog '{blog.Title}' has been published.",
                    type: NotificationType.Success,
                    action: NotificationAction.Published,
                    module: NotificationModule.Blog,
                    entityId: blog.Id,
                    currentUserId: currentUserId);
            }

            return result;
        }
        public async Task<bool> ArchiveAsync(
    int id,
    int currentUserId)
        {
            var blog = await _repository.GetByIdForUpdateAsync(id);
            if (blog == null)
                return false;

            var result =
                await _repository.ArchiveAsync(
                    id,
                    currentUserId);

            if (result)
            {
                await _sitemap.GenerateAsync();

                await _notificationService.CreateAsync(
                    title: "Blog Archived",
                    message: $"Blog '{blog.Title}' was archived.",
                    type: NotificationType.Warning,
                    action: NotificationAction.Archived,
                    module: NotificationModule.Blog,
                    entityId: blog.Id,
                    currentUserId: currentUserId);
            }

            return result;
        }
        public async Task<bool> RestoreAsync(
    int id,
    int currentUserId)
        {
            var blog = await _repository.GetByIdForUpdateAsync(id);

            if (blog == null)
                return false;

            var result =
                await _repository.RestoreAsync(
                    id,
                    currentUserId);

            if (result)
            {
                await _sitemap.GenerateAsync();

                await _notificationService.CreateAsync(
                    title: "Blog Restored",
                    message: $"Blog '{blog.Title}' was restored successfully.",
                    type: NotificationType.Success,
                    action: NotificationAction.Restored,
                    module: NotificationModule.Blog,
                    entityId: blog.Id,
                    currentUserId: currentUserId);
            }

            return result;
        }
        //public async Task<List<BlogResponseDto>> GetAllAsync()
        //{
        //    var blogs = await _repository.GetAllAsync();

        //    return blogs.Select(MapToDto).ToList();
        //}
        public async Task<PagedResponse<BlogResponseDto>> GetAllAsync(BlogQueryDto query)
        {
            var result = await _repository.GetAllAsync(query);

            return new PagedResponse<BlogResponseDto>
            {
               
                Data = result.Blogs,

                PageNumber = query.PageNumber,

                PageSize = query.PageSize,

                TotalRecords = result.TotalCount,

                TotalPages =
                    (int)Math.Ceiling(result.TotalCount / (double)query.PageSize)
            };
        }

        public async Task<List<BlogResponseDto>> GetPublishedAsync()
        {
            var blogs = await _repository.GetPublishedAsync();

            return blogs.Select(MapToDto).ToList();
        }

        public async Task<BlogResponseDto?> GetByIdAsync(int id)
        {
            var blog = await _repository.GetByIdAsync(id);

            if (blog == null)
                return null;

            return MapToDto(blog);
        }

        public async Task<BlogResponseDto?> GetBySlugAsync(string slug)
        {
            var blog = await _repository.GetBySlugAsync(slug);

            if (blog == null)
                return null;

            return MapToDto(blog);
        }

        public async Task<bool> IncrementOpenCountAsync(int id)
        {
            return await _repository.IncrementOpenCountAsync(id);
        }

        private static BlogResponseDto MapToDto(Blog blog)
        {
            return new BlogResponseDto
            {
                Id = blog.Id,

                Title = blog.Title,

                Category = blog.CategoryNavigation == null
                    ? null
                    : new BlogCategoryInfoDto
                    {
                        Id = blog.CategoryNavigation.Id,
                        Name = blog.CategoryNavigation.Name,
                        Slug = blog.CategoryNavigation.Slug
                    },

                 MetaDescription = blog.MetaDescription,

                ShortDescription = blog.ShortDescription,

                Slug = blog.Slug,

                CoverImage = blog.CoverImage,

                BlogDetails = blog.BlogDetails,

                Tags = blog.Tags,

                Status = blog.Status.ToString(),

                EntryDate = blog.CreatedAt,

                PublishingDate = blog.PublishingDate,

                ReadTime = blog.ReadTime,

                OpenCount = blog.OpenCount
            };
        }
        public async Task<bool> ScheduleAsync(
    ScheduleBlogDto dto,
    int currentUserId)
        {
            var blog = await _repository.GetByIdAsync(dto.BlogId);

            if (blog == null)
                return false;

            if (dto.PublishingDate <= DateTime.UtcNow)
                throw new Exception("Publishing date must be in the future.");

            blog.Status = BlogStatus.Scheduled;

            blog.PublishingDate = dto.PublishingDate;

            blog.UpdatedAt = DateTime.UtcNow;

            blog.UpdatedByUserId = currentUserId;

            await _repository.UpdateAsync(blog);

            return true;
        }
        public static class SlugHelper
        {
            public static string Generate(string text)
            {
                return text.Trim()
                           .ToLower()
                           .Replace(" ", "-");
            }
        }

        public async Task<BlogViewerResponseDto?> ViewAsync(string slug)
        {
            //---------------------------------------
            // Get Blog
            //---------------------------------------

            var blog =
                await _repository.GetViewerAsync(slug);

            if (blog == null)
                return null;

            //---------------------------------------
            // Increase Open Count
            //---------------------------------------

            await _repository.IncrementOpenCountAsync(blog.Id);

            blog.OpenCount++;

            //---------------------------------------
            // Related Blogs
            //---------------------------------------

            blog.RelatedBlogs =
       await _repository.GetRelatedBlogsAsync(
           blog.Id,
           blog.Category?.Id);

            return blog;
        }

        //public async Task<List<BlogCardDto>> GetBlogCardsAsync()
        //{
        //    return await _repository.GetBlogCardsAsync();
        //}
        public async Task<List<BlogCardDto>> GetBlogCardsAsync(
    string? categorySlug = null)
        {
            return await _repository.GetBlogCardsAsync(categorySlug);
        }
        private static int CalculateReadTime(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 1;

            var words = content.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries).Length;

            return Math.Max(1, (int)Math.Ceiling(words / 200.0));
        }
    }
}
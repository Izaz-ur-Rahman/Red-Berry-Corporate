using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces.Blog
{
    using BlogEntity = RedBerryCorporate.Models.Blog;
    public interface IBlogRepository
    {

        Task<BlogEntity> AddAsync(BlogEntity blog);

        Task<BlogEntity> UpdateAsync(BlogEntity blog);

        Task<bool> DeleteAsync(int id, int deletedByUserId);

        Task<BlogEntity?> GetByIdAsync(int id);

        Task<BlogEntity?> GetBySlugAsync(string slug);

        Task<(List<BlogEntity> Blogs, int TotalCount)> GetAllAsync(BlogQueryDto query);

        Task<List<BlogEntity>> GetPublishedAsync();

        Task<List<BlogEntity>> GetScheduledBlogsAsync();

        Task<bool> PublishAsync(int id, int publishedByUserId);

        Task<bool> ArchiveAsync(int id, int updatedByUserId);

        Task<bool> RestoreAsync(int id, int updatedByUserId);

        Task<bool> IncrementOpenCountAsync(int id);

        Task<bool> SlugExistsAsync(string slug, int? ignoreId = null);
    }
}
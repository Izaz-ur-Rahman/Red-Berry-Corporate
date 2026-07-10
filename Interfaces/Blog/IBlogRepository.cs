using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces.Blog
{
    public interface IBlogRepository
    {
        Task<Models.Blog> AddAsync(Models.Blog blog);

        Task<Models.Blog?> UpdateAsync(Models.Blog blog);

        Task<bool> DeleteAsync(int id);

        Task<Models.Blog?> GetByIdAsync(int id);

        Task<Models.Blog?> GetBySlugAsync(string slug);

        Task<(List<RedBerryCorporate.Models.Blog> Blogs, int TotalCount)> GetAllAsync(BlogQueryDto query);
        Task<List<Models.Blog>> GetPublishedAsync();

        Task<bool> IncrementOpenCountAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? ignoreId = null);
    }
}
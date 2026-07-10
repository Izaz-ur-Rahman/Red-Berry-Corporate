using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Common;

namespace RedBerryCorporate.Interfaces.Blog
{
    public interface IBlogService
    {
        Task<BlogResponseDto> AddAsync(CreateBlogDto dto);

        Task<BlogResponseDto?> UpdateAsync(UpdateBlogDto dto);

        Task<bool> DeleteAsync(int id);

        //Task<List<BlogResponseDto>> GetAllAsync();
        Task<PagedResponse<BlogResponseDto>> GetAllAsync(BlogQueryDto query);
        Task<List<BlogResponseDto>> GetPublishedAsync();

        Task<BlogResponseDto?> GetByIdAsync(int id);

        Task<BlogResponseDto?> GetBySlugAsync(string slug);

        Task<bool> IncrementOpenCountAsync(int id);
    }
}
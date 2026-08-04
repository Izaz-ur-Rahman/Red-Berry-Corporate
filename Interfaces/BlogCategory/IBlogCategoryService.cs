using RedBerryCorporate.DTOs.BlogCategory;

namespace RedBerryCorporate.Interfaces.BlogCategory
{
    public interface IBlogCategoryService
    {
        Task<BlogCategoryResponseDto> CreateAsync(
            CreateBlogCategoryDto dto);

        Task<List<BlogCategoryResponseDto>> GetAllAsync();

        Task<BlogCategoryResponseDto?> GetByIdAsync(int id);

        Task<BlogCategoryResponseDto?> UpdateAsync(
            UpdateBlogCategoryDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces.BlogCategory
{
    using BlogEntity = RedBerryCorporate.Models.BlogCategory;
    public interface IBlogCategoryRepository
    {
        Task<BlogEntity> CreateAsync(BlogEntity category);

        Task<BlogEntity?> GetByIdAsync(int id);

        Task<List<BlogEntity>> GetAllAsync();

        Task<BlogEntity> UpdateAsync(BlogEntity category);

        Task<bool> DeleteAsync(int id);

        Task<bool> NameExistsAsync(
            string name,
            int? ignoreId = null);
        Task<BlogEntity?> GetInactiveByNameAsync(string name);
    }
}
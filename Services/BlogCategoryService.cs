using RedBerryCorporate.DTOs.BlogCategory;
using RedBerryCorporate.Interfaces.BlogCategory;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class BlogCategoryService : IBlogCategoryService
    {
        private readonly IBlogCategoryRepository _repository;

        public BlogCategoryService(
            IBlogCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<BlogCategoryResponseDto> CreateAsync(
            CreateBlogCategoryDto dto)
        {
            var name = dto.Name.Trim();

            if (await _repository.NameExistsAsync(name))
            {
                throw new Exception(
                    "Blog category already exists.");
            }

            var category = new BlogCategory
            {
                Name = name,
                Slug = GenerateSlug(name),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            category =
                await _repository.CreateAsync(category);

            return MapToDto(category);
        }

        public async Task<List<BlogCategoryResponseDto>>
            GetAllAsync()
        {
            var categories =
                await _repository.GetAllAsync();

            return categories
                .Select(MapToDto)
                .ToList();
        }

        public async Task<BlogCategoryResponseDto?>
            GetByIdAsync(int id)
        {
            var category =
                await _repository.GetByIdAsync(id);

            if (category == null)
                return null;

            return MapToDto(category);
        }

        public async Task<BlogCategoryResponseDto?>
            UpdateAsync(UpdateBlogCategoryDto dto)
        {
            var category =
                await _repository.GetByIdAsync(dto.Id);

            if (category == null)
                return null;

            var name = dto.Name.Trim();

            if (await _repository.NameExistsAsync(
                name,
                dto.Id))
            {
                throw new Exception(
                    "Blog category already exists.");
            }

            category.Name = name;
            category.Slug = GenerateSlug(name);
            category.Description =
                dto.Description?.Trim();
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            category =
                await _repository.UpdateAsync(category);

            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static BlogCategoryResponseDto MapToDto(
            BlogCategory category)
        {
            return new BlogCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        private static string GenerateSlug(string text)
        {
            return text
                .Trim()
                .ToLower()
                .Replace(" ", "-");
        }
    }
}
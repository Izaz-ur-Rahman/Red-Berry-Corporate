using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.Interfaces.BlogCategory;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class BlogCategoryRepository
        : IBlogCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogCategoryRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BlogCategory> CreateAsync(
            BlogCategory category)
        {
            await _context.BlogCategories.AddAsync(category);

            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<BlogCategory?> GetByIdAsync(int id)
        {
            return await _context.BlogCategories
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<BlogCategory>> GetAllAsync()
        {
            return await _context.BlogCategories
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<BlogCategory> UpdateAsync(
            BlogCategory category)
        {
            _context.BlogCategories.Update(category);

            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category =
                await _context.BlogCategories
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return false;

            category.IsActive = false;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> NameExistsAsync(
       string name,
       int? ignoreId = null)
        {
            name = name.Trim();

            return await _context.BlogCategories
                .AnyAsync(x =>
                    x.Name.ToLower() == name.ToLower() &&
                    x.IsActive &&
                    (!ignoreId.HasValue ||
                     x.Id != ignoreId.Value));
        }

        public async Task<BlogCategory?> GetInactiveByNameAsync(string name)
        {
            name = name.Trim();

            return await _context.BlogCategories
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == name.ToLower() &&
                    !x.IsActive);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Blog> AddAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return blog;
        }

        public async Task<Blog?> UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            return blog;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
                return false;

            blog.IsActive = false;

            _context.Blogs.Update(blog);

            await _context.SaveChangesAsync();
           

            return true;
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .FirstOrDefaultAsync(x =>
    x.Id == id &&
    x.IsActive);
        }

        public async Task<Blog?> GetBySlugAsync(string slug)
        {
            return await _context.Blogs
               .FirstOrDefaultAsync(x =>
    x.Slug == slug &&
    x.IsActive);
        }

        //public async Task<List<Blog>> GetAllAsync()
        //{
        //    return await _context.Blogs
        //        .OrderByDescending(x => x.Id)
        //        .ToListAsync();
        //}
        public async Task<(List<Blog> Blogs, int TotalCount)> GetAllAsync(BlogQueryDto query)
        {
            IQueryable<Blog> blogs =
      _context.Blogs.Where(x => x.IsActive);

            //------------------------------------
            // Search
            //------------------------------------

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                blogs = blogs.Where(x =>
                    x.Title.Contains(query.Search));
            }

            //------------------------------------
            // Category
            //------------------------------------

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                blogs = blogs.Where(x =>
                    x.Category == query.Category);
            }

            //------------------------------------
            // Status
            //------------------------------------

            if (query.Status.HasValue)
            {
                blogs = blogs.Where(x =>
                    x.Status == query.Status);
            }

            //------------------------------------
            // Sorting
            //------------------------------------

            //blogs = query.SortBy.ToLower() == "oldest"
            //    ? blogs.OrderBy(x => x.EntryDate)
            //    : blogs.OrderByDescending(x => x.EntryDate);

            //------------------------------------
            // Total Count
            //------------------------------------

            int total = await blogs.CountAsync();

            //------------------------------------
            // Pagination
            //------------------------------------

            var data = await blogs
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (data, total);
        }
        public async Task<List<Blog>> GetPublishedAsync()
        {
            return await _context.Blogs
             .Where(x =>
    x.IsActive &&
    x.Status == BlogStatus.Published)
                .OrderByDescending(x => x.PublishingDate)
                .ToListAsync();
        }

        public async Task<bool> IncrementOpenCountAsync(int id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return false;

            blog.OpenCount++;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SlugExistsAsync(string slug, int? ignoreId = null)
        {
            slug = slug.ToLower().Trim();

            return await _context.Blogs.AnyAsync(x =>
                x.Slug.ToLower() == slug &&
                (!ignoreId.HasValue || x.Id != ignoreId));
        }
    }
}
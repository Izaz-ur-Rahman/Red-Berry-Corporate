using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
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

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Blog?> GetBySlugAsync(string slug)
        {
            return await _context.Blogs
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<List<Blog>> GetAllAsync()
        {
            return await _context.Blogs
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<Blog>> GetPublishedAsync()
        {
            return await _context.Blogs
                .Where(x => x.Status == BlogStatus.Published)
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
    }
}
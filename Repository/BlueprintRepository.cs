using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class BlueprintRepository : IBlueprintRepository
    {
        private readonly ApplicationDbContext _context;

        public BlueprintRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BlueprintSubmission> CreateAsync(BlueprintSubmission blueprint)
        {
            _context.BlueprintSubmissions.Add(blueprint);

            await _context.SaveChangesAsync();

            return blueprint;
        }

        public async Task<BlueprintSubmission?> GetByIdAsync(int id)
        {
            return await _context.BlueprintSubmissions
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(List<BlueprintSubmission>, int)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search)
        {
            IQueryable<BlueprintSubmission> query =
                _context.BlueprintSubmissions;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Company.Contains(search) ||
                    x.Email.Contains(search));
            }

            int total = await query.CountAsync();

            List<BlueprintSubmission> data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }

        public async Task UpdateAsync(BlueprintSubmission blueprint)
        {
            _context.BlueprintSubmissions.Update(blueprint);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlueprintSubmission blueprint)
        {
            _context.BlueprintSubmissions.Remove(blueprint);

            await _context.SaveChangesAsync();
        }
    }
}
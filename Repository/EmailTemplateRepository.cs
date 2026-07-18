using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.EmailTemplate;
using RedBerryCorporate.Interfaces.EmailTemplate;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class EmailTemplateRepository
        : IEmailTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailTemplateRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmailTemplate> AddAsync(
    EmailTemplate template)
        {
            await _context.EmailTemplates.AddAsync(template);

            await _context.SaveChangesAsync();

            return template;
        }
        public async Task<EmailTemplate?> UpdateAsync(
    EmailTemplate template)
        {
            _context.EmailTemplates.Update(template);

            await _context.SaveChangesAsync();

            return template;
        }
        public async Task<bool> DeleteAsync(
    int id,
    int deletedByUserId)
        {
            var template =
                await _context.EmailTemplates
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);

            if (template == null)
                return false;

            template.IsActive = false;

            template.UpdatedAt = DateTime.UtcNow;

            template.UpdatedByUserId = deletedByUserId;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<EmailTemplate?> GetByIdAsync(
    int id)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);
        }
        public async Task<EmailTemplate?> GetByTemplateKeyAsync(
    string templateKey)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(x =>
                    x.TemplateKey == templateKey &&
                    x.IsActive);
        }
        public async Task<bool> TemplateKeyExistsAsync(
    string templateKey,
    int? ignoreId = null)
        {
            templateKey = templateKey
                .Trim()
                .ToUpper();

            return await _context.EmailTemplates.AnyAsync(x =>
                x.TemplateKey == templateKey &&
                x.IsActive &&
                (!ignoreId.HasValue ||
                 x.Id != ignoreId));
        }

        public async Task<(List<EmailTemplate> Templates,
    int TotalCount)>
GetAllAsync(EmailTemplateQueryDto query)
        {
            IQueryable<EmailTemplate> templates =
                _context.EmailTemplates
                .Where(x => x.IsActive);

            //-----------------------------------
            // Search
            //-----------------------------------

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                templates = templates.Where(x =>
                    x.Name.Contains(query.Search) ||
                    x.TemplateKey.Contains(query.Search));
            }

            //-----------------------------------
            // Sorting
            //-----------------------------------

            templates =
                query.SortBy.ToLower() == "oldest"
                ? templates.OrderBy(x => x.CreatedAt)
                : templates.OrderByDescending(x => x.CreatedAt);

            //-----------------------------------
            // Count
            //-----------------------------------

            int total =
                await templates.CountAsync();

            //-----------------------------------
            // Pagination
            //-----------------------------------

            var data =
                await templates
                    .Skip((query.PageNumber - 1) *
                          query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

            return (data, total);
        }

    }
}
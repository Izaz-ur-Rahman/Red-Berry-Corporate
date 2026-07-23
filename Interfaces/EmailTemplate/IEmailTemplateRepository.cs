using RedBerryCorporate.DTOs.EmailTemplate;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces.EmailTemplate
{
    public interface IEmailTemplateRepository
    {
        Task<Models.EmailTemplate> AddAsync(Models.EmailTemplate template);

        Task<Models.EmailTemplate?> UpdateAsync(Models.EmailTemplate template);

        Task<bool> DeleteAsync(
            int id,
            int deletedByUserId);

        Task<Models.EmailTemplate?> GetByIdAsync(int id);

        Task<Models.EmailTemplate?> GetByTemplateKeyAsync(string templateKey);

        Task<(List<Models.EmailTemplate> Templates, int TotalCount)>
            GetAllAsync(EmailTemplateQueryDto query);

        Task<bool> TemplateKeyExistsAsync(
            string templateKey,
            int? ignoreId = null);
       // Task<Models.EmailTemplate?> GetByTemplateKeyAsync(string templateKey);
    }
}
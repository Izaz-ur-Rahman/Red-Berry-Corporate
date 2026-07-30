using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.DTOs.EmailTemplate;

namespace RedBerryCorporate.Interfaces.EmailTemplate
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplateResponseDto> AddAsync(
            CreateEmailTemplateDto dto,
            int currentUserId);

        Task<EmailTemplateResponseDto?> UpdateAsync(
            UpdateEmailTemplateDto dto,
            int currentUserId);

        Task<bool> DeleteAsync(
            int id,
            int currentUserId);

        Task<EmailTemplateResponseDto?> GetByIdAsync(
            int id);

        Task<PagedResponse<EmailTemplateResponseDto>>
            GetAllAsync(EmailTemplateQueryDto query);
        // NEW
        Task<RenderedEmailDto> RenderAsync(
            string templateKey,
            object model);
    }
}
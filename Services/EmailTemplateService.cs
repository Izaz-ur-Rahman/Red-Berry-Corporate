using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.DTOs.EmailTemplate;
using RedBerryCorporate.Interfaces.EmailTemplate;
using RedBerryCorporate.Models;
using System.Reflection;

namespace RedBerryCorporate.Services
{
    public class EmailTemplateService
        : IEmailTemplateService
    {
        private readonly IEmailTemplateRepository _repository;

        public EmailTemplateService(
            IEmailTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmailTemplateResponseDto> AddAsync(
    CreateEmailTemplateDto dto,
    int currentUserId)
        {
            var templateKey =
                await GenerateTemplateKeyAsync(dto.Name);

            var template = new EmailTemplate
            {
                Name = dto.Name,

                Subject = dto.Subject,

                BodyHtml = dto.BodyHtml,

                TemplateKey = templateKey,

                IsActive = true,

                CreatedAt = DateTime.UtcNow,

                CreatedByUserId = currentUserId
            };

            template = await _repository.AddAsync(template);

            return MapToDto(template);
        }

        public async Task<EmailTemplateResponseDto?> UpdateAsync(
    UpdateEmailTemplateDto dto,
    int currentUserId)
        {
            var template =
                await _repository.GetByIdAsync(dto.Id);

            if (template == null)
                return null;

            template.Name = dto.Name;

            template.Subject = dto.Subject;

            template.BodyHtml = dto.BodyHtml;

            template.UpdatedAt = DateTime.UtcNow;

            template.UpdatedByUserId = currentUserId;

            template = await _repository.UpdateAsync(template);

            return MapToDto(template);
        }
        public async Task<bool> DeleteAsync(
    int id,
    int currentUserId)
        {
            return await _repository.DeleteAsync(
                id,
                currentUserId);
        }

        public async Task<EmailTemplateResponseDto?> GetByIdAsync(
    int id)
        {
            var template =
                await _repository.GetByIdAsync(id);

            if (template == null)
                return null;

            return MapToDto(template);
        }

        public async Task<PagedResponse<EmailTemplateResponseDto>>
    GetAllAsync(
        EmailTemplateQueryDto query)
        {
            var result =
                await _repository.GetAllAsync(query);

            return new PagedResponse<EmailTemplateResponseDto>
            {
                Data = result.Templates
                    .Select(MapToDto)
                    .ToList(),

                PageNumber = query.PageNumber,

                PageSize = query.PageSize,

                TotalRecords = result.TotalCount,

                TotalPages =
                    (int)Math.Ceiling(
                        result.TotalCount /
                        (double)query.PageSize)
            };
        }

        private static EmailTemplateResponseDto
    MapToDto(
        EmailTemplate template)
        {
            return new EmailTemplateResponseDto
            {
                Id = template.Id,

                Name = template.Name,

                TemplateKey = template.TemplateKey,

                Subject = template.Subject,

                BodyHtml = template.BodyHtml,

                IsActive = template.IsActive,

                CreatedAt = template.CreatedAt,

                UpdatedAt = template.UpdatedAt
            };
        }

        private async Task<string>
GenerateTemplateKeyAsync(
    string name)
        {
            var baseKey =
                name.Trim()
                    .ToUpper()
                    .Replace(" ", "_")
                    .Replace("-", "_");

            var key = baseKey;

            int counter = 1;

            while (await _repository
                .TemplateKeyExistsAsync(key))
            {
                key = $"{baseKey}_{counter}";
                counter++;
            }

            return key;
        }
        public async Task<RenderedEmailDto> RenderAsync(
    string templateKey,
    object model)
        {
            var template =
                await _repository.GetByTemplateKeyAsync(templateKey);

            if (template == null)
                throw new Exception(
                    $"Email template '{templateKey}' not found.");

            string subject = template.Subject;
            string body = template.BodyHtml;

            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                string token = "{{" + property.Name + "}}";

                string value =
                    property.GetValue(model)?.ToString() ?? "";

                subject = subject.Replace(token, value);

                body = body.Replace(token, value);
            }

            return new RenderedEmailDto
            {
                Subject = subject,
                Body = body
            };
        }
    }
}
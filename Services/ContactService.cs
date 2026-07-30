using RedBerryCorporate.DTOs.Contact;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Interfaces.Notification;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly ICaptchaService _captchaService;
        public ContactService(
            IContactRepository repository,
            INotificationService notificationService,
            ICaptchaService captchaService)
        {
            _repository = repository;
            _notificationService = notificationService;
            _captchaService = captchaService;
        }

        public async Task<ContactResponseDto> CreateAsync(ContactCreateDto dto)
        {
            bool verified =
    await _captchaService.VerifyTokenAsync(dto.CaptchaToken);

            if (!verified)
            {
                throw new Exception("Captcha verification failed.");
            }
            var contact = new Contact
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone.Trim(),
                Company = string.IsNullOrWhiteSpace(dto.Company) ? null : dto.Company.Trim(),
                Interest = dto.Interest.Trim(),
                Message = dto.Message.Trim(),
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _repository.CreateAsync(contact);
            await _notificationService.CreateAsync(
    title: "New Contact Inquiry",
    message: $"A new inquiry has been submitted by '{result.Name}'.",
    type: NotificationType.Info,
    action: NotificationAction.Created,
    module: NotificationModule.Contact,
    entityId: result.Id,
    currentUserId: null
);
            return MapToResponse(result);
        }

        public async Task<List<ContactResponseDto>> GetAllAsync()
        {
            var contacts = await _repository.GetAllAsync();

            return contacts
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<ContactResponseDto?> GetByIdAsync(int id)
        {
            var contact = await _repository.GetByIdAsync(id);

            if (contact == null)
                return null;

            return MapToResponse(contact);
        }

        public async Task<bool> UpdateAsync(ContactUpdateDto dto)
        {
            var contact = await _repository.GetByIdAsync(dto.Id);

            if (contact == null)
                return false;

            contact.Name = dto.Name.Trim();
            contact.Email = dto.Email.Trim();
            contact.Phone = dto.Phone.Trim();
            contact.Company = string.IsNullOrWhiteSpace(dto.Company) ? null : dto.Company.Trim();
            contact.Interest = dto.Interest.Trim();
            contact.Message = dto.Message.Trim();
            contact.UpdatedDate = DateTime.UtcNow;


            //return await _repository.UpdateAsync(contact);
            bool result = await _repository.UpdateAsync(contact);

            if (result)
            {
                await _notificationService.CreateAsync(
                    title: "Contact Updated",
                    message: $"Contact '{contact.Name}' was updated.",
                    type: NotificationType.Info,
                    action: NotificationAction.Updated,
                    module: NotificationModule.Contact,
                    entityId: contact.Id,
                    currentUserId: null
                );
            }

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            //return await _repository.DeleteAsync(id);
            var contact = await _repository.GetByIdAsync(id);

            if (contact == null)
                return false;

            bool result = await _repository.DeleteAsync(id);

            if (result)
            {
                await _notificationService.CreateAsync(
                    title: "Contact Deleted",
                    message: $"Contact '{contact.Name}' was deleted.",
                    type: NotificationType.Warning,
                    action: NotificationAction.Deleted,
                    module: NotificationModule.Contact,
                    entityId: contact.Id,
                    currentUserId: null
                );
            }

            return result;
        }

        public async Task<(List<ContactResponseDto> Data, int TotalRecords)> GetPagedAsync(ContactListRequestDto request)
        {
            var result = await _repository.GetPagedAsync(request);

            return
            (
                result.Data.Select(MapToResponse).ToList(),
                result.TotalRecords
            );
        }

        private static ContactResponseDto MapToResponse(Contact contact)
        {
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Company = contact.Company,
                Interest = contact.Interest,
                Message = contact.Message,
                CreatedDate = contact.CreatedDate
            };
        }
    }
}
using RedBerryCorporate.DTOs.Contact;
using RedBerryCorporate.Models;
using RedBerryCorporate.Repository;

namespace RedBerryCorporate.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task<ContactResponseDto> CreateAsync(ContactCreateDto dto)
        {
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

            return await _repository.UpdateAsync(contact);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
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
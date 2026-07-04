using RedBerryCorporate.DTOs.Contact;

namespace RedBerryCorporate.Services
{
    public interface IContactService
    {
        Task<ContactResponseDto> CreateAsync(ContactCreateDto dto);

        Task<List<ContactResponseDto>> GetAllAsync();

        Task<ContactResponseDto?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(ContactUpdateDto dto);

        Task<bool> DeleteAsync(int id);

        Task<(List<ContactResponseDto> Data, int TotalRecords)> GetPagedAsync(ContactListRequestDto request);
    }
}
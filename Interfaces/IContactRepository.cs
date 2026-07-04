using RedBerryCorporate.DTOs.Contact;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact> CreateAsync(Contact contact);

        Task<List<Contact>> GetAllAsync();

        Task<Contact?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(Contact contact);

        Task<bool> DeleteAsync(int id);

        Task<(List<Contact> Data, int TotalRecords)> GetPagedAsync(ContactListRequestDto request);
    }
}
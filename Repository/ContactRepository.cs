using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.Contact;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;


namespace RedBerryCorporate.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<bool> UpdateAsync(Contact contact)
        {
            _context.Contacts.Update(contact);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (contact == null)
                return false;

            contact.IsDeleted = true;
            contact.UpdatedDate = DateTime.UtcNow;

            _context.Contacts.Update(contact);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(List<Contact> Data, int TotalRecords)> GetPagedAsync(ContactListRequestDto request)
        {
            IQueryable<Contact> query = _context.Contacts
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                string search = request.Search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.Phone.Contains(search) ||
                    (x.Company ?? "").Contains(search) ||
                    x.Interest.Contains(search));
            }

            int totalRecords = await query.CountAsync();

            List<Contact> contacts = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (contacts, totalRecords);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region User

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> UserNameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(x => x.UserName == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Employee

        public async Task<TblEmployee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.TblEmployees
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<TblEmployee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.TblEmployees
                .FirstOrDefaultAsync(x => x.EMAIL_ADDRESS == email);
        }

        public async Task<TblEmployee> CreateEmployeeAsync(TblEmployee employee)
        {
            await _context.TblEmployees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task UpdateEmployeeAsync(TblEmployee employee)
        {
            _context.TblEmployees.Update(employee);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
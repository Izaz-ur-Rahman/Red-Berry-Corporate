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

        #region Get All Users

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }

        #endregion

        #region Get User By Id

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        #endregion

        #region Get User By Username

        public async Task<User?> GetByUserNameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username);
        }

        #endregion

        #region Username Exists

        public async Task<bool> UserNameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(x => x.UserName == username);
        }

        #endregion

        #region Employee Exists

        public async Task<bool> EmployeeExistsAsync(int empId)
        {
            return await _context.TblEmployees
                .AnyAsync(x => x.ID == empId);
        }

        #endregion

        #region Create User

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        #endregion

        #region Update User

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Delete User

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
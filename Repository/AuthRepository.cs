using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await
            (
                from u in _context.Users
                join e in _context.TblEmployees
                    on u.EmpId equals e.ID
                where
                    e.EMAIL_ADDRESS == email &&
                    u.IsActive == true
                select u
            ).FirstOrDefaultAsync();
        }
        //public async Task<User?> GetUserByUserNameAsync(string userName)
        //{
        //    return await _context.Users
        //        .FirstOrDefaultAsync(x =>
        //            x.UserName == userName &&
        //            x.IsActive == true);
        //}

        public async Task<TblEmployee?> GetEmployeeAsync(int empId)
        {
            return await _context.TblEmployees
                .FirstOrDefaultAsync(x => x.ID == empId);
        }
    }
}
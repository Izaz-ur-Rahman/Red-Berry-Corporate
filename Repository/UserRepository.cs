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
                .Where(x => x.IsActive == true)
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

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
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

        public async Task CreateEmployeeAsync(TblEmployee employee)
        {
            await _context.TblEmployees.AddAsync(employee);
        }

        public void UpdateEmployee(TblEmployee employee)
        {
            _context.TblEmployees.Update(employee);
        }

        #endregion

        #region Joined Data

        public async Task<List<(User User, TblEmployee? Employee)>> GetUsersWithEmployeesAsync()
        {
            var result = await
                (from user in _context.Users
                 where user.IsActive == true

                 join employee in _context.TblEmployees
                 on user.EmpId equals employee.ID
                 into employeeGroup

                 from employee in employeeGroup.DefaultIfEmpty()

                 orderby user.ID descending

                 select new
                 {
                     User = user,
                     Employee = employee
                 }).ToListAsync();

            return result
                .Select(x => (x.User, x.Employee))
                .ToList();
        }

        public async Task<(User User, TblEmployee? Employee)?> GetUserWithEmployeeAsync(int userId)
        {
            var result = await
                (from user in _context.Users

                 join employee in _context.TblEmployees
                 on user.EmpId equals employee.ID
                 into employeeGroup

                 from employee in employeeGroup.DefaultIfEmpty()

                 where user.ID == userId
                 && user.IsActive == true

                 select new
                 {
                     User = user,
                     Employee = employee
                 }).FirstOrDefaultAsync();

            if (result == null)
                return null;

            return (result.User, result.Employee);
        }
        public async Task<(User User, TblEmployee? Employee)?> GetProfileAsync(int userId)
        {
            var result =
                await
                (
                    from user in _context.Users

                    join employee in _context.TblEmployees
                    on user.EmpId equals employee.ID
                    into employeeGroup

                    from employee in employeeGroup.DefaultIfEmpty()

                    where user.ID == userId
                          && user.IsActive == true

                    select new
                    {
                        User = user,
                        Employee = employee
                    }
                ).FirstOrDefaultAsync();

            if (result == null)
                return null;

            return (result.User, result.Employee);
        }

        public async Task<User?> GetUserByEmployeeIdAsync(int employeeId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.EmpId == employeeId &&
                    x.IsActive == true);
        }
        #endregion

        #region SaveChanges

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
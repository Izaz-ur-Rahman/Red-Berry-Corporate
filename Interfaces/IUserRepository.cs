using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IUserRepository
    {
        #region User

        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByUserNameAsync(string username);

        Task<bool> UserNameExistsAsync(string username);

        Task CreateAsync(User user);

        void Update(User user);

        void Delete(User user);

        #endregion

        #region Employee

        Task<TblEmployee?> GetEmployeeByIdAsync(int id);

        Task<TblEmployee?> GetEmployeeByEmailAsync(string email);

        Task CreateEmployeeAsync(TblEmployee employee);

        void UpdateEmployee(TblEmployee employee);

        #endregion

        #region Joined Data

        Task<List<(User User, TblEmployee? Employee)>> GetUsersWithEmployeesAsync();

        Task<(User User, TblEmployee? Employee)?> GetUserWithEmployeeAsync(int userId);

        #endregion

        #region Save

        Task SaveChangesAsync();

        #endregion
    }
}
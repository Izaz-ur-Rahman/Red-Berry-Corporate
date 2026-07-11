using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByUserNameAsync(string username);

        Task<bool> UserNameExistsAsync(string username);

        Task<bool> EmployeeExistsAsync(int empId);

        Task<User> CreateAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);
    }
}
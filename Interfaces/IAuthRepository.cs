using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUserNameAsync(string userName);

        Task<TblEmployee?> GetEmployeeAsync(int empId);
    }
}
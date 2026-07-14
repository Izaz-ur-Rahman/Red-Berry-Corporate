using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IAuthRepository
    {
        //Task<User?> GetUserByUserNameAsync(string userName);
        Task<User?> GetUserByEmailAsync(string email);
        Task<TblEmployee?> GetEmployeeAsync(int empId);
    }
}
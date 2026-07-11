using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IUserRepository
    {
        // User Table
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUserNameAsync(string username);
        Task<bool> UserNameExistsAsync(string username);

        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);

        // Employee Table
        Task<TblEmployee?> GetEmployeeByIdAsync(int id);
        Task<TblEmployee> CreateEmployeeAsync(TblEmployee employee);
        Task UpdateEmployeeAsync(TblEmployee employee);
        Task<TblEmployee?> GetEmployeeByEmailAsync(string email);
    }
}
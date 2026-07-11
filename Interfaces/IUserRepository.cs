using RedBerryCorporate.DTOs.User;

namespace RedBerryCorporate.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserListDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(CreateUserDto dto, int createdBy);

        Task<bool> UpdateAsync(UpdateUserDto dto, int updatedBy);

        Task<bool> DeleteAsync(int id);

        Task<bool> UserNameExistsAsync(string email);
    }
}
using RedBerryCorporate.DTOs.User;

namespace RedBerryCorporate.Interfaces
{
    public interface IUserService
    {
        Task<List<UserListDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateUserDto dto, int createdBy);

        Task UpdateAsync(UpdateUserDto dto, int updatedBy);

        Task DeleteAsync(int id, int deletedBy);
    }
}
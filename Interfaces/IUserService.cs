using RedBerryCorporate.DTOs.User;

namespace RedBerryCorporate.Interfaces
{
    public interface IUserService
    {
        Task<List<UserListDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateUserDto dto, int createdBy);

        Task UpdateAsync(UpdateUserDto dto, int updatedBy);
        Task<ProfileDto?> GetProfileAsync(int userId);
        Task DeleteAsync(int id, int deletedBy);
    }
}
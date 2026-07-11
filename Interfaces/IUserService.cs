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
        Task UpdateProfileAsync(UpdateProfileDto dto, int currentUserId);
        Task DeleteAsync(int id, int deletedBy);
        Task ChangePasswordAsync(
    int userId,
    ChangePasswordDto dto);
    }
}
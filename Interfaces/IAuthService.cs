using RedBerryCorporate.DTOs.Auth;

namespace RedBerryCorporate.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}
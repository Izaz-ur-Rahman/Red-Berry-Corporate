using RedBerryCorporate.DTOs.Auth;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;
using System.Linq;

namespace RedBerryCorporate.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IAuthRepository repository,
            JwtHelper jwtHelper)
        {
            _repository = repository;
            _jwtHelper = jwtHelper;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            // Find User
            var user = await _repository.GetUserByUserNameAsync(dto.UserName);

            if (user == null)
            {
                throw new Exception("Invalid username or password.");
            }

            // Verify Password
            bool isPasswordValid = false;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                // New PBKDF2 Password
                if (user.Password.Contains(":"))
                {
                    isPasswordValid =
                        EncryptionHelper.VerifyPassword(
                            dto.Password,
                            user.Password);
                }
                else
                {
                    // Legacy SHA1 Password
                    var encryptedPassword =
                        EncryptionHelper.EncrptPassword(dto.Password);

                    isPasswordValid =
                        encryptedPassword == user.Password;
                }
            }

            if (!isPasswordValid)
            {
                throw new Exception("Invalid username or password.");
            }

            // Employee Details
            TblEmployee? employee = null;

            if (user.EmpId.HasValue)
            {
                employee =
                    await _repository.GetEmployeeAsync(user.EmpId.Value);
            }

            // Generate JWT Token
            var jwt = _jwtHelper.GenerateToken(
       user,
       employee?.FULL_NAME ?? "");

            return new LoginResponseDto
            {
                UserId = user.ID,
                UserName = user.UserName ?? "",
                EmpId = user.EmpId,
                EmployeeName = employee?.FULL_NAME,
                Role = user.RoleNames,
                Token = jwt.Token,
                Expiry = jwt.Expiry
            };
        }
    }
}
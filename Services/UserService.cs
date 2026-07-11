using RedBerryCorporate.DTOs.User;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region Get All Users

        public async Task<List<UserListDto>> GetAllAsync()
        {
            var records = await _userRepository.GetUsersWithEmployeesAsync();

            return records.Select(x => new UserListDto
            {
                Id = x.User.ID,
                UserName = x.User.UserName,
                Name = x.Employee?.FULL_NAME,
                Email = x.Employee?.EMAIL_ADDRESS,
                Role = x.User.RoleNames,
                IsActive = x.User.IsActive ?? false,
                ProfileImage = x.Employee?.Photo
            }).ToList();
        }

        #endregion

        #region Get User By Id

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var record = await _userRepository.GetUserWithEmployeeAsync(id);

            if (record == null)
                return null;

            return new UserDto
            {
                Id = record.Value.User.ID,
                EmpId = record.Value.User.EmpId ?? 0,

                UserName = record.Value.User.UserName,

                Name = record.Value.Employee?.FULL_NAME,

                Email = record.Value.Employee?.EMAIL_ADDRESS,

                MobileNo = record.Value.Employee?.MOBILE_M,

                WhatsappNo = record.Value.Employee?.WhatsappNo,

                Languages = record.Value.Employee?.Languages,

                Role = record.Value.User.RoleNames,

                IsActive = record.Value.User.IsActive ?? false,

                ProfileImage = record.Value.Employee?.Photo
            };
        }

        #endregion
        #region Get My Profile

        public async Task<ProfileDto?> GetProfileAsync(int userId)
        {
            // Get User + Employee
            var record = await _userRepository.GetProfileAsync(userId);

            if (record == null)
                return null;

            return new ProfileDto
            {
                UserId = record.Value.User.ID,

                EmployeeId = record.Value.User.EmpId ?? 0,

                UserName = record.Value.User.UserName,

                Name = record.Value.Employee?.FULL_NAME,

                Email = record.Value.Employee?.EMAIL_ADDRESS,

                MobileNo = record.Value.Employee?.MOBILE_SMS,

                WhatsappNo = record.Value.Employee?.WhatsappNo,

                Position = record.Value.Employee?.Position,

                Bio = record.Value.Employee?.Bio,

                Languages = record.Value.Employee?.Languages,

                Facebook = record.Value.Employee?.Facebook,

                LinkedIn = record.Value.Employee?.LinkedIn,

                Twitter = record.Value.Employee?.Twitter,

                ProfileImage = record.Value.Employee?.Photo,

                IsActive = record.Value.User.IsActive ?? false
            };
        }

        #endregion
        #region Update Profile

        public async Task UpdateProfileAsync(UpdateProfileDto dto, int currentUserId)
        {
            // ===========================
            // Get Logged-in User
            // ===========================

            var user = await _userRepository.GetByIdAsync(currentUserId);

            if (user == null)
                throw new Exception("User not found.");

            if (!user.EmpId.HasValue)
                throw new Exception("Employee record not found.");

            // ===========================
            // Get Employee
            // ===========================

            var employee =
                await _userRepository.GetEmployeeByIdAsync(user.EmpId.Value);

            if (employee == null)
                throw new Exception("Employee not found.");

            // ===========================
            // Check Email Duplicate
            // ===========================

            if (!string.Equals(employee.EMAIL_ADDRESS, dto.Email,
                StringComparison.OrdinalIgnoreCase))
            {
                var existingEmployee =
                    await _userRepository.GetEmployeeByEmailAsync(dto.Email);

                if (existingEmployee != null &&
                    existingEmployee.ID != employee.ID)
                {
                    throw new Exception("Email already exists.");
                }
            }

            // ===========================
            // Update Employee Information
            // ===========================

            employee.FULL_NAME = dto.Name;
            employee.EMAIL_ADDRESS = dto.Email;

            employee.MOBILE_SMS = dto.MobileNo;
            employee.WhatsappNo = dto.WhatsappNo;

            employee.Bio = dto.Bio;
            employee.LinkedIn = dto.LinkedIn;
            employee.Facebook = dto.Facebook;
            employee.Twitter = dto.Twitter;

            employee.Languages = dto.Languages;

            employee.UpdatedBy = currentUserId;
            employee.UpdatedDate = DateTime.UtcNow;

            _userRepository.UpdateEmployee(employee);

            await _userRepository.SaveChangesAsync();
        }

        #endregion
        #region Change Password

        public async Task ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto)
        {
            // ============================
            // Get User
            // ============================

            var user =
                await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            // ============================
            // Verify Old Password
            // ============================

            bool passwordMatched = false;

            // New PBKDF2 Password
            if (!string.IsNullOrWhiteSpace(user.Password)
                && user.Password.Contains(":"))
            {
                passwordMatched =
                    EncryptionHelper.VerifyPassword(
                        dto.OldPassword,
                        user.Password);
            }
            else
            {
                // Legacy SHA1 Password
                passwordMatched =
                    EncryptionHelper.EncrptPassword(dto.OldPassword)
                    == user.Password;
            }

            if (!passwordMatched)
                throw new Exception("Old password is incorrect.");

            // ============================
            // Prevent Same Password
            // ============================

            if (dto.OldPassword == dto.NewPassword)
                throw new Exception("New password must be different from old password.");

            // ============================
            // Save New Password
            // ============================

            user.Password =
                EncryptionHelper.HashPassword(dto.NewPassword);

            user.UpdatedDate = DateTime.UtcNow;
            user.UpdatedBy = userId;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync();
        }

        #endregion
        #region Create User



        public async Task CreateAsync(CreateUserDto dto, int createdBy)
        {
            // Check duplicate username
            if (await _userRepository.UserNameExistsAsync(dto.UserName))
                throw new Exception("Username already exists.");

            // Check duplicate employee email
            var existingEmployee =
                await _userRepository.GetEmployeeByEmailAsync(dto.Email);

            if (existingEmployee != null)
                throw new Exception("Email already exists.");

            // ============================
            // Create Employee
            // ============================

            var employee = new TblEmployee
            {
                FULL_NAME = dto.Name,
                EMAIL_ADDRESS = dto.Email,
                MOBILE_SMS = dto.MobileNo,
                WhatsappNo = dto.WhatsappNo,
                Languages = dto.Languages,

                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsActive = dto.IsActive
            };

            await _userRepository.CreateEmployeeAsync(employee);

            // Save first so Employee ID is generated
            await _userRepository.SaveChangesAsync();

            // ============================
            // Create User
            // ============================

            var user = new User
            {
                UserName = dto.UserName,

                Password = EncryptionHelper.HashPassword(dto.Password),

                EmpId = employee.ID,

                RoleNames = dto.Role,

                CreatedBy = createdBy,

                CreatedDate = DateTime.UtcNow,

                IsActive = dto.IsActive
            };

            await _userRepository.CreateAsync(user);

            await _userRepository.SaveChangesAsync();
        }

        #endregion
    

        #region Update User

        public async Task UpdateAsync(UpdateUserDto dto, int updatedBy)
        {
            // ============================
            // Get Existing User
            // ============================

            var user = await _userRepository.GetByIdAsync(dto.Id);

            if (user == null)
                throw new Exception("User not found.");

            // ============================
            // Check Username
            // ============================

            if (!string.Equals(user.UserName, dto.UserName, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userRepository.UserNameExistsAsync(dto.UserName))
                    throw new Exception("Username already exists.");
            }

            // ============================
            // Get Employee
            // ============================

            if (!user.EmpId.HasValue)
                throw new Exception("Employee record not found.");

            var employee = await _userRepository.GetEmployeeByIdAsync(user.EmpId.Value);

            if (employee == null)
                throw new Exception("Employee not found.");

            // ============================
            // Check Email
            // ============================

            var emailEmployee = await _userRepository.GetEmployeeByEmailAsync(dto.Email);

            if (emailEmployee != null &&
                emailEmployee.ID != employee.ID)
            {
                throw new Exception("Email already exists.");
            }

            // ============================
            // Update Employee
            // ============================

            employee.FULL_NAME = dto.Name;
            employee.EMAIL_ADDRESS = dto.Email;
            employee.MOBILE_SMS = dto.MobileNo;
            employee.WhatsappNo = dto.WhatsappNo;
            employee.Languages = dto.Languages;

            employee.UpdatedBy = updatedBy;
            employee.UpdatedDate = DateTime.UtcNow;

            employee.IsActive = dto.IsActive;

            _userRepository.UpdateEmployee(employee);

            // ============================
            // Update User
            // ============================

            user.UserName = dto.UserName;

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = EncryptionHelper.HashPassword(dto.Password);
            }

            user.RoleNames = dto.Role;

            user.IsActive = dto.IsActive;

            user.UpdatedBy = updatedBy;
            user.UpdatedDate = DateTime.UtcNow;

            _userRepository.Update(user);

            // ============================
            // Save
            // ============================

            await _userRepository.SaveChangesAsync();
        }

        #endregion



        #region Delete User

        public async Task DeleteAsync(int id, int deletedBy)
        {
            // Get User

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found.");

            // Get Employee

            if (user.EmpId.HasValue)
            {
                var employee =
                    await _userRepository.GetEmployeeByIdAsync(user.EmpId.Value);

                if (employee != null)
                {
                    employee.IsActive = false;
                    employee.UpdatedBy = deletedBy;
                    employee.UpdatedDate = DateTime.UtcNow;

                    _userRepository.UpdateEmployee(employee);
                }
            }

            // Disable User

            user.IsActive = false;
            user.UpdatedBy = deletedBy;
            user.UpdatedDate = DateTime.UtcNow;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync();
        }

        #endregion
    }
}
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

        #region Create User

        #region Create User

        public async Task CreateAsync(CreateUserDto dto, int createdBy)
        {
            // Check duplicate username
            if (await _repository.UserNameExistsAsync(dto.UserName))
                throw new Exception("Username already exists.");

            // Check duplicate employee email
            var existingEmployee =
                await _repository.GetEmployeeByEmailAsync(dto.Email);

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

            await _repository.CreateEmployeeAsync(employee);

            // Save first so Employee ID is generated
            await _repository.SaveChangesAsync();

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

            await _repository.CreateAsync(user);

            await _repository.SaveChangesAsync();
        }

        #endregion
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
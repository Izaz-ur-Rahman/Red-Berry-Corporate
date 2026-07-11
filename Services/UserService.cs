using RedBerryCorporate.DTOs.User;
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

        public async Task CreateAsync(CreateUserDto dto, int createdBy)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update User

        public async Task UpdateAsync(UpdateUserDto dto, int updatedBy)
        {
            throw new NotImplementedException();
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
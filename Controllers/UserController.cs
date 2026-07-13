using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.User;
using RedBerryCorporate.Interfaces;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Get All Users

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(new
            {
                Success = true,
                Message = "Users retrieved successfully.",
                Data = users
            });
        }

        #endregion

        #region Get User By Id

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "User retrieved successfully.",
                Data = user
            });
        }

        #endregion
        #region My Profile

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            var profile = await _userService.GetProfileAsync(currentUserId);

            if (profile == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Profile not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = profile
            });
        }

        #endregion
        #region Update My Profile
        [Authorize]
        [HttpPost("profile/update")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            await _userService.UpdateProfileAsync(dto, currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "Profile updated successfully."
            });
        }
        //[Authorize]
        //[HttpPut("Profile")]
        //public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        //{
        //    var currentUserId = GetCurrentUserIdOrThrow();

        //    await _userService.UpdateProfileAsync(dto, currentUserId);

        //    return Ok(new
        //    {
        //        Success = true,
        //        Message = "Profile updated successfully."
        //    });
        //}

        #endregion
        #region Change Password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            await _userService.ChangePasswordAsync(currentUserId, dto);

            return Ok(new
            {
                Success = true,
                Message = "Password changed successfully."
            });
        }

        //[HttpPut("change-password")]
        //public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        //{
        //    var currentUserId = GetCurrentUserIdOrThrow();

        //    await _userService.ChangePasswordAsync(currentUserId, dto);

        //    return Ok(new
        //    {
        //        Success = true,
        //        Message = "Password changed successfully."
        //    });
        //}

        #endregion
        #region Create User
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            //var currentUserId = GetCurrentUserIdOrThrow();
            var currentUserId = 1;

            await _userService.CreateAsync(dto, currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "User created successfully."
            });
        }

        #endregion

        #region Update User
        [HttpPost("update")]
        public async Task<IActionResult> Update(UpdateUserDto dto)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            await _userService.UpdateAsync(dto, currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "User updated successfully."
            });
        }
        //[HttpPut]
        //public async Task<IActionResult> Update(UpdateUserDto dto)
        //{
        //    var currentUserId = GetCurrentUserIdOrThrow();

        //    await _userService.UpdateAsync(dto, currentUserId);

        //    return Ok(new
        //    {
        //        Success = true,
        //        Message = "User updated successfully."
        //    });
        //}

        #endregion

        #region Delete User

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            await _userService.DeleteAsync(id, currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "User deleted successfully."
            });
        }

        #endregion
        #region Upload Profile Image

        [HttpPost("profile-image")]
        public async Task<IActionResult> UploadProfileImage(
            [FromForm] UploadProfileImageDto dto)
        {
            // Temporary until JWT is enabled
            // var currentUserId = GetCurrentUserIdOrThrow();

            var currentUserId = 1;

            await _userService.UploadProfileImageAsync(
                dto,
                currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "Profile image uploaded successfully."
            });
        }

        #endregion
    }
}
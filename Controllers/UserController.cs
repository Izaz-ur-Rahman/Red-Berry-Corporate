using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.User;
using RedBerryCorporate.Interfaces;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        #region Create User

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            await _userService.CreateAsync(dto, currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "User created successfully."
            });
        }

        #endregion

        #region Update User

        [HttpPut]
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

        #endregion

        #region Delete User

        [HttpDelete("{id}")]
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
    }
}
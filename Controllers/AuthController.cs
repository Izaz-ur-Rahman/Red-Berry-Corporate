using Microsoft.AspNetCore.Mvc;
using RedBerryCorporate.DTOs.Auth;
using RedBerryCorporate.Interfaces;

namespace RedBerryCorporate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        /// <summary>
        /// User Login
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _service.LoginAsync(dto);

            return Ok(new
            {
                Success = true,
                Message = "Login Successful",
                Data = result
            });
        }
    }
}
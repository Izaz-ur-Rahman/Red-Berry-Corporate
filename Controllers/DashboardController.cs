using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.DTOs.Dashboard;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.Dashboard;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("Stats")]
        public async Task<IActionResult> Stats()
        {
            var result =
                await _dashboardService.GetDashboardAsync();

            return Ok(new ApiResponse<DashboardResponseDto>
            {
                Success = true,
                Message = "Dashboard statistics retrieved successfully.",
                Data = result
            });
        }
    }
}
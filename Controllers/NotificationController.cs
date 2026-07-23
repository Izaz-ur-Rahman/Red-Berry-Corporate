using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.Notification;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.Notification;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _service;

        public NotificationController(
            INotificationService service)
        {
            _service = service;
        }

        #region Get Notifications

        [HttpGet("List")]
        public async Task<IActionResult> GetAll(
            [FromQuery] NotificationQueryDto query)
        {
            var result =
                await _service.GetAllAsync(query);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Notifications retrieved successfully.",
                Data = result
            });
        }

        #endregion

        #region Get Notification By Id

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Notification not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Notification retrieved successfully.",
                Data = result
            });
        }

        #endregion

        #region Unread Count

        [HttpGet("UnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count =
                await _service.GetUnreadCountAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Unread notification count retrieved successfully.",
                Data = new
                {
                    Count = count
                }
            });
        }

        #endregion

        #region Mark As Read

        [HttpPost("MarkRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result =
                await _service.MarkAsReadAsync(id);

            if (!result)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Notification not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Notification marked as read."
            });
        }

        #endregion

        #region Mark All Read

        [HttpPost("MarkAllRead")]
        public async Task<IActionResult> MarkAllRead()
        {
            var count =
                await _service.MarkAllAsReadAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "All notifications marked as read.",
                Data = new
                {
                    UpdatedRecords = count
                }
            });
        }

        #endregion
    }
}
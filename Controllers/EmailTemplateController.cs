using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.EmailTemplate;
using RedBerryCorporate.Interfaces.EmailTemplate;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailTemplateController : BaseApiController
    {
        private readonly IEmailTemplateService _service;

        public EmailTemplateController(
            IEmailTemplateService service)
        {
            _service = service;
        }
        #region Add Template

        [HttpPost("Add")]
        public async Task<IActionResult> Add(
            CreateEmailTemplateDto dto)
        {
            var currentUserId =
                GetCurrentUserIdOrThrow();

            var result =
                await _service.AddAsync(
                    dto,
                    currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "Email template created successfully.",
                Data = result
            });
        }

        #endregion
        #region Update Template

        [HttpPost("Update")]
        public async Task<IActionResult> Update(
            UpdateEmailTemplateDto dto)
        {
            var currentUserId =
                GetCurrentUserIdOrThrow();

            var result =
                await _service.UpdateAsync(
                    dto,
                    currentUserId);

            if (result == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Template not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Email template updated successfully.",
                Data = result
            });
        }

        #endregion
        #region Delete Template

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var currentUserId =
                GetCurrentUserIdOrThrow();

            var result =
                await _service.DeleteAsync(
                    id,
                    currentUserId);

            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Template not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Template deleted successfully."
            });
        }

        #endregion

        #region Get Templates

        [HttpGet("List")]
        public async Task<IActionResult> GetAll(
            [FromQuery]
    EmailTemplateQueryDto query)
        {
            var result =
                await _service.GetAllAsync(query);

            return Ok(result);
        }

        #endregion

        #region Get Template By Id

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Template not found."
                });
            }

            return Ok(result);
        }

        #endregion

    }
}
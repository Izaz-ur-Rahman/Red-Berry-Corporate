using Microsoft.AspNetCore.Mvc;
using RedBerryCorporate.DTOs.Contact;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Services;

namespace RedBerryCorporate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactController(IContactService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ContactCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Validation failed.",
                    ModelState));
            }

            var result = await _service.CreateAsync(dto);

            return Ok(new ApiResponse<ContactResponseDto>(
                true,
                "Contact submitted successfully.",
                result));
        }

        [HttpPost("list")]
        public async Task<IActionResult> List(ContactListRequestDto dto)
        {
            var result = await _service.GetPagedAsync(dto);

            return Ok(new ApiResponse<object>(
                true,
                "Contacts retrieved successfully.",
                new
                {
                    result.TotalRecords,
                    result.Data
                }));
        }

        [HttpPost("details")]
        public async Task<IActionResult> Details(ContactDeleteDto dto)
        {
            var contact = await _service.GetByIdAsync(dto.Id);

            if (contact == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Contact not found.",
                    null));
            }

            return Ok(new ApiResponse<ContactResponseDto>(
                true,
                "Contact retrieved successfully.",
                contact));
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(ContactUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Validation failed.",
                    ModelState));
            }

            bool updated = await _service.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Contact not found.",
                    null));
            }

            return Ok(new ApiResponse<object>(
                true,
                "Contact updated successfully.",
                null));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(ContactDeleteDto dto)
        {
            bool deleted = await _service.DeleteAsync(dto.Id);

            if (!deleted)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Contact not found.",
                    null));
            }

            return Ok(new ApiResponse<object>(
                true,
                "Contact deleted successfully.",
                null));
        }
    }
}
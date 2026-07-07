using Microsoft.AspNetCore.Mvc;
using RedBerryCorporate.DTOs.Contact;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces;

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
        public async Task<IActionResult> Create([FromBody] ContactCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ValidationHelper.GetErrors(ModelState)
                });
            }

            var result = await _service.CreateAsync(dto);

            return StatusCode(StatusCodes.Status201Created,
     new ApiResponse<ContactResponseDto>
     {
         Success = true,
         Message = "Contact submitted successfully.",
         Data = result
     });
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] ContactListRequestDto dto)
        {
            var result = await _service.GetPagedAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Contacts retrieved successfully.",
                Data = new
                {
                    result.TotalRecords,
                    result.Data
                }
            });
        }
        [HttpPost("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var contact = await _service.GetByIdAsync(id);

            if (contact == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Contact not found."
                });
            }

            return Ok(new ApiResponse<ContactResponseDto>
            {
                Success = true,
                Message = "Contact retrieved successfully.",
                Data = contact
            });
        }

        [HttpPost("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContactUpdateDto dto)
        {
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ValidationHelper.GetErrors(ModelState)
                });
            }

            bool updated = await _service.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Contact not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Contact updated successfully."
            });
        }

        [HttpPost("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Contact not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Contact deleted successfully."
            });
        }
    }
}
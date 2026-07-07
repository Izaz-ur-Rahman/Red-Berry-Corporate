using Microsoft.AspNetCore.Mvc;
using RedBerryCorporate.DTOs.Blueprint;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces;

namespace RedBerryCorporate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlueprintController : ControllerBase
    {
        private readonly IBlueprintService _service;

        public BlueprintController(IBlueprintService service)
        {
            _service = service;
        }

        #region Create Blueprint

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BlueprintCreateDto dto)
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
                new ApiResponse<BlueprintResponseDto>
                {
                    Success = true,
                    Message = "Blueprint submitted successfully.",
                    Data = result
                });
        }

        #endregion

        #region List (CMS)

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] BlueprintListRequestDto dto)
        {
            var result = await _service.GetPagedAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Blueprints retrieved successfully.",
                Data = new
                {
                    result.TotalRecords,
                    result.Data
                }
            });
        }

        #endregion

        #region Details

        [HttpPost("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var blueprint = await _service.GetByIdAsync(id);

            if (blueprint == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Blueprint not found."
                });
            }

            return Ok(new ApiResponse<BlueprintResponseDto>
            {
                Success = true,
                Message = "Blueprint retrieved successfully.",
                Data = blueprint
            });
        }

        #endregion

        #region Update

        [HttpPost("update/{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] BlueprintUpdateDto dto)
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
                    Message = "Blueprint not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Blueprint updated successfully."
            });
        }

        #endregion

        #region Delete

        [HttpPost("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Blueprint not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Blueprint deleted successfully."
            });
        }

        #endregion
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.BlogCategory;
using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.BlogCategory;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BlogCategoryController : BaseApiController
    {
        private readonly IBlogCategoryService _blogCategoryService;

        public BlogCategoryController(
            IBlogCategoryService blogCategoryService)
        {
            _blogCategoryService = blogCategoryService;
        }

        #region Add Blog Category

        [HttpPost("Add")]
        public async Task<IActionResult> Add(
            [FromBody] CreateBlogCategoryDto dto)
        {
            var result =
                await _blogCategoryService.CreateAsync(dto);

            return Ok(new ApiResponse<BlogCategoryResponseDto>
            {
                Success = true,
                Message = "Blog category created successfully.",
                Data = result
            });
        }

        #endregion

        #region Get All Blog Categories

        [AllowAnonymous]
        [HttpGet("List")]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _blogCategoryService.GetAllAsync();

            return Ok(new ApiResponse<List<BlogCategoryResponseDto>>
            {
                Success = true,
                Message = "Blog categories retrieved successfully.",
                Data = result
            });
        }

        #endregion

        #region Get Blog Category By Id

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result =
                await _blogCategoryService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Blog category not found."
                });
            }

            return Ok(new ApiResponse<BlogCategoryResponseDto>
            {
                Success = true,
                Message = "Blog category retrieved successfully.",
                Data = result
            });
        }

        #endregion

        #region Update Blog Category

        [HttpPost("Update")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateBlogCategoryDto dto)
        {
            var result =
                await _blogCategoryService.UpdateAsync(dto);

            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Blog category not found."
                });
            }

            return Ok(new ApiResponse<BlogCategoryResponseDto>
            {
                Success = true,
                Message = "Blog category updated successfully.",
                Data = result
            });
        }

        #endregion

        #region Delete Blog Category

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _blogCategoryService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Blog category not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Blog category deleted successfully."
            });
        }

        #endregion
    }
}

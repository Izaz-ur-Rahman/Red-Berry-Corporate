using Microsoft.AspNetCore.Mvc;
using RedBerryApi.Controllers;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.Interfaces.Blog;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : BaseApiController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        #region Add Blog

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm] CreateBlogDto dto)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            // Until JWT is enabled you may temporarily use:
            // var currentUserId = 1;

            var result = await _blogService.AddAsync(
                dto,
                currentUserId);

            return Ok(new
            {
                Success = true,
                Message = "Blog created successfully.",
                Data = result
            });
        }

        #endregion

        #region Update Blog

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] UpdateBlogDto dto)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            // Temporary
            // var currentUserId = 1;

            var result = await _blogService.UpdateAsync(
                dto,
                currentUserId);
            if (result == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Blog updated successfully.",
                Data = result
            });
        }

        #endregion

        #region Delete Blog

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            // Temporary
            // var currentUserId = 1;

            var result = await _blogService.DeleteAsync(
                id,
                currentUserId);
            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Blog deleted successfully."
            });
        }

        #endregion
        #region Publish Blog

        [HttpPost("Publish/{id}")]
        public async Task<IActionResult> Publish(int id)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            // Temporary
            // var currentUserId = 1;

            var result = await _blogService.PublishAsync(
                id,
                currentUserId);
            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Blog published successfully."
            });
        }

        #endregion
        #region Archive Blog

        [HttpPost("Archive/{id}")]
        public async Task<IActionResult> Archive(int id)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            // Temporary
            // var currentUserId = 1;

            var result = await _blogService.ArchiveAsync(
                id,
                currentUserId);
            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Blog archived successfully."
            });
        }

        #endregion
        #region Restore Blog

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            // Temporary
            // var currentUserId = 1;

            var result = await _blogService.RestoreAsync(
                id,
                currentUserId);
            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Blog restored successfully."
            });
        }

        #endregion
        #region Schedule Blog

        [HttpPost("Schedule")]
        public async Task<IActionResult> Schedule(ScheduleBlogDto dto)
        {
            var result = await _blogService.ScheduleAsync(dto);

            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Blog scheduled successfully."
            });
        }

        #endregion
        #region Get All Blogs (Pagination)

        [HttpGet("List")]
        public async Task<IActionResult> GetAll([FromQuery] BlogQueryDto query)
        {
            var result = await _blogService.GetAllAsync(query);

            return Ok(result);
        }

        #endregion

        #region Published Blogs

        [HttpGet("Published")]
        public async Task<IActionResult> GetPublished()
        {
            var result = await _blogService.GetPublishedAsync();

            return Ok(result);
        }

        #endregion

        #region Get Blog By Id

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _blogService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(result);
        }

        #endregion

        #region Get Blog By Slug

        [HttpGet("Slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _blogService.GetBySlugAsync(slug);

            if (result == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(result);
        }

        #endregion

        #region Increment Open Count

        [HttpPost("OpenCount/{id}")]
        public async Task<IActionResult> IncrementOpenCount(int id)
        {
            var result = await _blogService.IncrementOpenCountAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Blog not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Open count updated."
            });
        }

        #endregion
    }
}
using Microsoft.AspNetCore.Mvc;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.Interfaces.Blog;

namespace RedBerryCorporate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
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
            try
            {
                var result = await _blogService.AddAsync(dto);

                return Ok(new
                {
                    Success = true,
                    Message = "Blog created successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        #endregion

        #region Update Blog

        // POST used intentionally (instead of PUT)
        // because some hosting/server proxy configurations
        // block PUT requests.

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] UpdateBlogDto dto)
        {
            try
            {
                var result = await _blogService.UpdateAsync(dto);

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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        #endregion

        #region Delete Blog

        // POST used intentionally
        // because DELETE sometimes causes proxy/server issues.

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _blogService.DeleteAsync(id);

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
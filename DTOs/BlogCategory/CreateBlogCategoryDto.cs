using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.BlogCategory
{
    public class CreateBlogCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
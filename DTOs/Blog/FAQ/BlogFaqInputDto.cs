using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.Blog.FAQ
{
    public class BlogFaqInputDto
    {
        [Required]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
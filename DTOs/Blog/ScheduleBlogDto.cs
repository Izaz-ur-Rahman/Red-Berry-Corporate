using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.Blog
{
    public class ScheduleBlogDto
    {
        [Required]
        public int BlogId { get; set; }

        [Required]
        public DateTime PublishingDate { get; set; }
    }
}
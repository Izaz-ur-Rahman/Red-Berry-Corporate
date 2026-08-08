using Microsoft.AspNetCore.Http;
using RedBerryCorporate.DTOs.Blog.FAQ;
using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.Blog
{
    public class CreateBlogDto
    {
        [Required]
        public string Title { get; set; }

        public string? Slug { get; set; }

        //public string? Category { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string? MetaDescription { get; set; }
        [MaxLength(300)]
        public string? ShortDescription { get; set; }
        public IFormFile? CoverImage { get; set; }

        public string? BlogDetails { get; set; }

        public string? Tags { get; set; }

        public DateTime? PublishingDate { get; set; }
        //public List<BlogFaqInputDto> FAQs { get; set; } = new();
        public string? FAQs { get; set; }
    }
}
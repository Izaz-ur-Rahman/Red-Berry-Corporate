using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.Blog
{
    public class CreateBlogDto
    {
        [Required]
        public string Title { get; set; }

        public string Category { get; set; }

        public string MetaDescription { get; set; }

        public string BlogDetails { get; set; }

        // Optional
        public string? Slug { get; set; }

        public string Tags { get; set; }

        // Upload file instead of string
        public IFormFile? CoverImage { get; set; }
    }
}
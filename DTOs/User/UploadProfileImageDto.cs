using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.User
{
    public class UploadProfileImageDto
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
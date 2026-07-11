using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.User
{
    public class CreateUserDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        public string MobileNo { get; set; }

        public string WhatsappNo { get; set; }

        public string Languages { get; set; }

        public string ReraNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
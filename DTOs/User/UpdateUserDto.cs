using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string MobileNo { get; set; }

        public string WhatsappNo { get; set; }

        public string Languages { get; set; }

        public string ReraNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
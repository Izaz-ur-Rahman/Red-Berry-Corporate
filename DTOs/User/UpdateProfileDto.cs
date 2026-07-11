using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.User
{
    public class UpdateProfileDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string MobileNo { get; set; }

        [MaxLength(20)]
        public string WhatsappNo { get; set; }

        [MaxLength(100)]
        public string Position { get; set; }

        [MaxLength(1000)]
        public string Bio { get; set; }

        [MaxLength(200)]
        public string Languages { get; set; }

        [MaxLength(300)]
        public string Facebook { get; set; }

        [MaxLength(300)]
        public string LinkedIn { get; set; }

        [MaxLength(300)]
        public string Twitter { get; set; }

        // Store the image path or filename.
        // Later we'll support actual file uploads.
        public string ProfileImage { get; set; }
    }
}
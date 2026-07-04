using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.Contact
{
    public class ContactCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(40, MinimumLength = 5)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(120)]
        public string? Company { get; set; }

        [Required]
        [StringLength(80)]
        public string Interest { get; set; } = string.Empty;

        [Required]
        [StringLength(1500, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;
    }
}
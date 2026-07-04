using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedBerryCorporate.Models
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? Company { get; set; }

        [Required]
        [MaxLength(80)]
        public string Interest { get; set; } = string.Empty;

        [Required]
        [MaxLength(1500)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
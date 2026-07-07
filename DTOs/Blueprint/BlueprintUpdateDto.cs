using System.ComponentModel.DataAnnotations;
using RedBerryCorporate.Enums;

namespace RedBerryCorporate.DTOs.Blueprint
{
    public class BlueprintUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Whatsapp { get; set; } = string.Empty;

        [Required]
        public string Company { get; set; } = string.Empty;

        public string? Location { get; set; }

        public BusinessStage BusinessStage { get; set; }

        public ReviewMethod ReviewMethod { get; set; }

        public string? Message { get; set; }

        [Required]
        public BlueprintResultDto Result { get; set; } = new();
    }
}
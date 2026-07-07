using System.ComponentModel.DataAnnotations;
using RedBerryCorporate.Enums;

namespace RedBerryCorporate.DTOs.Blueprint
{
    public class BlueprintCreateDto
    {
        #region Contact

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Whatsapp { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Company { get; set; } = string.Empty;

        public string? Location { get; set; }

        public BusinessStage BusinessStage { get; set; }

        public ReviewMethod ReviewMethod { get; set; }

        public string? Message { get; set; }

        #endregion

        #region Blueprint Result

        [Required]
        public BlueprintResultDto Result { get; set; } = new();

        #endregion
    }
}
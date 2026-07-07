using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RedBerryCorporate.Enums;

namespace RedBerryCorporate.Models
{
    public class BlueprintSubmission
    {
        [Key]
        public int Id { get; set; }

        #region Contact Information

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Whatsapp { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Company { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        public BusinessStage BusinessStage { get; set; }

        public ReviewMethod ReviewMethod { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Message { get; set; }

        #endregion

        #region Blueprint Result

        [MaxLength(250)]
        public string? Ambition { get; set; }

        public int TotalScore { get; set; }

        public int CorporateScore { get; set; }

        public int FinancialScore { get; set; }

        public int MarketScore { get; set; }

        public int LegacyScore { get; set; }

        [MaxLength(100)]
        public string? StrongestLayer { get; set; }

        [MaxLength(100)]
        public string? ExposedLayer { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ImprovementLayers { get; set; }

        [MaxLength(250)]
        public string? RecommendedPathway { get; set; }

        [MaxLength(200)]
        public string? OverallStatus { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? PatternsJson { get; set; }

        #endregion

        #region Audit

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #endregion
    }
}
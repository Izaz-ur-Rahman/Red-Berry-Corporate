using RedBerryCorporate.Enums;

namespace RedBerryCorporate.DTOs.Blueprint
{
    public class BlueprintResultDto
    {
        public string? Ambition { get; set; }

        public int TotalScore { get; set; }

        public int CorporateScore { get; set; }

        public int FinancialScore { get; set; }

        public int MarketScore { get; set; }

        public int LegacyScore { get; set; }

        public string? StrongestLayer { get; set; }

        public string? ExposedLayer { get; set; }

        public List<string>? ImprovementLayers { get; set; }

        public string? RecommendedPathway { get; set; }

        public string? OverallStatus { get; set; }

        public List<BlueprintPatternDto>? Patterns { get; set; }
    }
}
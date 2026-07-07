using RedBerryCorporate.Enums;

namespace RedBerryCorporate.DTOs.Blueprint
{
    public class BlueprintResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Whatsapp { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string? Location { get; set; }

        public BusinessStage BusinessStage { get; set; }

        public ReviewMethod ReviewMethod { get; set; }

        public string? Message { get; set; }

        public BlueprintResultDto Result { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
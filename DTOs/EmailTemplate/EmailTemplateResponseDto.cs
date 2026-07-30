namespace RedBerryCorporate.DTOs.EmailTemplate
{
    public class EmailTemplateResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string TemplateKey { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string BodyHtml { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
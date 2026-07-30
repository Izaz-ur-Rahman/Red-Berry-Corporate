namespace RedBerryCorporate.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string TemplateKey { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string BodyHtml { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedByUserId { get; set; }
    }
}
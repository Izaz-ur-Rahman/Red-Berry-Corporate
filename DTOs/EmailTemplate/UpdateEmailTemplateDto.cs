using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.EmailTemplate
{
    public class UpdateEmailTemplateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string BodyHtml { get; set; } = null!;
    }
}
namespace RedBerryCorporate.DTOs.Contact
{
    public class ContactResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Company { get; set; }

        public string Interest { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
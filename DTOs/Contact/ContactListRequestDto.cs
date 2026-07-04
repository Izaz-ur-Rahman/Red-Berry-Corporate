namespace RedBerryCorporate.DTOs.Contact
{
    public class ContactListRequestDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
    }
}
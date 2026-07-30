namespace RedBerryCorporate.DTOs.EmailTemplate
{
    public class EmailTemplateQueryDto
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "Newest";
    }
}
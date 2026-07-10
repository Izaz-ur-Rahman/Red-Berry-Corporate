using RedBerryCorporate.Enums;

namespace RedBerryCorporate.DTOs.Blog
{
    public class BlogQueryDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string? Category { get; set; }

        public BlogStatus? Status { get; set; }

        // Newest / Oldest
        public string SortBy { get; set; } = "Newest";
    }
}
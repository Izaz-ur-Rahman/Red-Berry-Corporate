namespace RedBerryCorporate.DTOs.Dashboard
{
    public class DashboardResponseDto
    {
        public int PublishedBlogs { get; set; }

        public int DraftBlogs { get; set; }

        public int ScheduledBlogs { get; set; }

        public int ArchivedBlogs { get; set; }

        public int ContactSubmissions { get; set; }

        public int BlueprintSubmissions { get; set; }
    }
}
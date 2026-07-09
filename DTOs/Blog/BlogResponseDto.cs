namespace RedBerryCorporate.DTOs.Blog
{
    public class BlogResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string MetaDescription { get; set; }

        public string Slug { get; set; }

        public string CoverImage { get; set; }

        public string BlogDetails { get; set; }

        public DateTime? PublishingDate { get; set; }

        public int? ReadTime { get; set; }

        public int? OpenCount { get; set; }

        public string Tags { get; set; }

        public string Status { get; set; }
    }
}

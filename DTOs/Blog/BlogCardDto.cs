namespace RedBerryCorporate.DTOs.Blog.Cards
{
    public class BlogCardDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Slug { get; set; }

        public string? CoverImage { get; set; }

        public string? ShortDescription { get; set; }

        public DateTime? PublishingDate { get; set; }

        public int ReadTime { get; set; }

        public BlogCardAuthorDto Author { get; set; } = new();
    }
}
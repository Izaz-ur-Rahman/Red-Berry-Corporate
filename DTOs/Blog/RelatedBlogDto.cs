namespace RedBerryCorporate.DTOs.Blog.Viewer
{
    public class RelatedBlogDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string CoverImage { get; set; }

        public DateTime? PublishingDate { get; set; }

        public int ReadTime { get; set; }
    }
}
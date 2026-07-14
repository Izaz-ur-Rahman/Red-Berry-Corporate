using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Blog.Viewer;

public class BlogDetailsDto
{
    public BlogResponseDto Blog { get; set; }

    public BlogAuthorDto Author { get; set; }

    public List<BlogResponseDto> RelatedBlogs { get; set; }
}
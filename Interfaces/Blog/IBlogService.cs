using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Blog.Cards;
using RedBerryCorporate.DTOs.Blog.Viewer;
using RedBerryCorporate.DTOs.Common;

namespace RedBerryCorporate.Interfaces.Blog
{
    public interface IBlogService
    {
        Task<BlogResponseDto> AddAsync(
            CreateBlogDto dto,
            int currentUserId);

        Task<BlogResponseDto?> UpdateAsync(
            UpdateBlogDto dto,
            int currentUserId);

        Task<bool> DeleteAsync(
            int id,
            int currentUserId);

        Task<bool> PublishAsync(
            int id,
            int currentUserId);

        Task<bool> ArchiveAsync(
            int id,
            int currentUserId);

        Task<bool> RestoreAsync(
            int id,
            int currentUserId);

        Task<PagedResponse<BlogResponseDto>> GetAllAsync(
            BlogQueryDto query);

        Task<List<BlogResponseDto>> GetPublishedAsync();

        Task<BlogResponseDto?> GetByIdAsync(int id);

        Task<BlogResponseDto?> GetBySlugAsync(string slug);

        Task<bool> IncrementOpenCountAsync(int id);
        Task<bool> ScheduleAsync(
    ScheduleBlogDto dto,
    int currentUserId);

        Task<BlogViewerResponseDto?> ViewAsync(string slug);
        Task<List<BlogCardDto>> GetBlogCardsAsync();

    }
}
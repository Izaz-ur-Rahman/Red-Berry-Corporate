using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Blog.Cards;
using RedBerryCorporate.DTOs.Blog.Viewer;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Blog> AddAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return blog;
        }

        public async Task<Blog?> UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            return blog;
        }

        public async Task<bool> DeleteAsync(int id, int deletedByUserId)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return false;

            blog.IsDeleted = true;

            blog.IsActive = false;

            blog.DeletedAt = DateTime.UtcNow;

            blog.DeletedByUserId = deletedByUserId;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> PublishAsync(int id, int publishedByUserId)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return false;

            blog.Status = BlogStatus.Published;

            blog.PublishingDate = DateTime.UtcNow;

            blog.PublishedAt = DateTime.UtcNow;

            blog.PublishedByUserId = publishedByUserId;

            blog.UpdatedAt = DateTime.UtcNow;

            blog.UpdatedByUserId = publishedByUserId;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ArchiveAsync(int id, int updatedByUserId)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return false;

            blog.Status = BlogStatus.Archived;

            blog.UpdatedAt = DateTime.UtcNow;

            blog.UpdatedByUserId = updatedByUserId;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RestoreAsync(int id, int updatedByUserId)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return false;

            blog.IsDeleted = false;

            blog.IsActive = true;

            blog.DeletedAt = null;

            blog.DeletedByUserId = null;

            blog.UpdatedAt = DateTime.UtcNow;

            blog.UpdatedByUserId = updatedByUserId;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<Blog>> GetScheduledBlogsAsync()
        {
            return await _context.Blogs
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.Status == BlogStatus.Scheduled &&
                    x.PublishingDate <= DateTime.UtcNow)
                .ToListAsync();
        }
        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Blogs
      .FirstOrDefaultAsync(x =>
          x.Id == id &&
          !x.IsDeleted &&
          x.IsActive);
        }

        public async Task<Blog?> GetBySlugAsync(string slug)
        {
            return await _context.Blogs
      .FirstOrDefaultAsync(x =>
          x.Slug == slug &&
          !x.IsDeleted &&
          x.IsActive &&
          x.Status == BlogStatus.Published);
        }

        //public async Task<List<Blog>> GetAllAsync()
        //{
        //    return await _context.Blogs
        //        .OrderByDescending(x => x.Id)
        //        .ToListAsync();
        //}
        public async Task<(List<BlogResponseDto> Blogs, int TotalCount)> GetAllAsync(BlogQueryDto query)
        {
            //      IQueryable<Blog> blogs =
            //_context.Blogs.Where(x => !x.IsDeleted && x.IsActive);
            var blogs =
          from blog in _context.Blogs

          join user in _context.Users
              on blog.CreatedByUserId equals user.ID

          join employee in _context.TblEmployees
              on user.EmpId equals employee.ID

          where !blog.IsDeleted &&
                blog.IsActive

          select new BlogResponseDto
          {
              Id = blog.Id,

              Title = blog.Title,

              Category = blog.Category,

              MetaDescription = blog.MetaDescription,
              ShortDescription = blog.ShortDescription,

              Slug = blog.Slug,

              CoverImage = blog.CoverImage,

              BlogDetails = blog.BlogDetails,

              Tags = blog.Tags,

              Status = blog.Status.ToString(),

              EntryDate = blog.CreatedAt,

              PublishingDate = blog.PublishingDate,

              ReadTime = blog.ReadTime,

              OpenCount = blog.OpenCount,

              Author = new BlogCardAuthorDto
              {
                  Name = employee.FULL_NAME,

                  Designation = employee.Position,

                  ProfileImage =
                      !string.IsNullOrWhiteSpace(employee.Photo)
                          ? employee.Photo
                          : employee.ProfilePicName
              }
          };
            //------------------------------------
            // Search
            //------------------------------------

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                blogs = blogs.Where(x =>
                    x.Title.Contains(query.Search));
            }

            //------------------------------------
            // Category
            //------------------------------------

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                blogs = blogs.Where(x =>
                    x.Category == query.Category);
            }

            //------------------------------------
            // Status
            //------------------------------------
            if (query.Status.HasValue)
            {
                blogs = blogs.Where(x =>
                    x.Status == query.Status.ToString());
            }
            //if (query.Status.HasValue)
            //{
            //    blogs = blogs.Where(x =>
            //        x.Status == query.Status);
            //}

            //------------------------------------
            // Sorting
            //------------------------------------
            blogs = query.SortBy.ToLower() == "oldest"
    ? blogs.OrderBy(x => x.EntryDate)
    : blogs.OrderByDescending(x => x.EntryDate);
            //       blogs = query.SortBy.ToLower() == "oldest"
            //? blogs.OrderBy(x => x.CreatedAt)
            //: blogs.OrderByDescending(x => x.CreatedAt);

            //------------------------------------
            // Total Count
            //------------------------------------

            int total = await blogs.CountAsync();

            //------------------------------------
            // Pagination
            //------------------------------------

            var data = await blogs
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (data, total);
        }
        public async Task<List<Blog>> GetPublishedAsync()
        {
            return await _context.Blogs
     .Where(x =>
         !x.IsDeleted &&
         x.IsActive &&
         x.Status == BlogStatus.Published)
     .OrderByDescending(x => x.PublishingDate)
     .ToListAsync();
        }

        public async Task<bool> IncrementOpenCountAsync(int id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return false;

            blog.OpenCount++;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SlugExistsAsync(string slug, int? ignoreId = null)
        {
            slug = slug.ToLower().Trim();

            return await _context.Blogs.AnyAsync(x =>
     !x.IsDeleted &&
     x.Slug.ToLower() == slug &&
     (!ignoreId.HasValue || x.Id != ignoreId));
        }

        public async Task PublishScheduledBlogAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task<BlogViewerResponseDto?> GetViewerAsync(string slug)
        {
            var data = await
            (
                from blog in _context.Blogs.AsNoTracking()

                join user in _context.Users
                    on blog.CreatedByUserId equals user.ID

                join employee in _context.TblEmployees
                    on user.EmpId equals employee.ID

                where blog.Slug == slug
                    && blog.Status == BlogStatus.Published
                    && blog.IsDeleted == false
                    && blog.IsActive

                select new BlogViewerResponseDto
                {
                    Id = blog.Id,

                    Title = blog.Title,

                    Slug = blog.Slug,

                    Category = blog.Category,

                    CoverImage = blog.CoverImage,

                    MetaDescription = blog.MetaDescription,

                    BlogDetails = blog.BlogDetails,

                    Tags = blog.Tags,

                    ReadTime = blog.ReadTime,

                    OpenCount = blog.OpenCount,

                    PublishingDate = blog.PublishingDate,

                    Author = new BlogAuthorDto
                    {
                        UserId = user.ID,

                        FullName = employee.FULL_NAME,

                        Designation = employee.Position,

                        Bio = employee.Bio,

                        Email = employee.EMAIL_ADDRESS,

                        Phone = employee.MOBILE_SMS,

                        LinkedIn = employee.LinkedIn,

                        Facebook = employee.Facebook,

                        Twitter = employee.Twitter,

                        Whatsapp = employee.WhatsappNo,

                        ProfileImage =
                            !string.IsNullOrWhiteSpace(employee.Photo)
                                ? employee.Photo
                                : employee.ProfilePicName
                    }
                }
            )
            .FirstOrDefaultAsync();

            return data;
        }

        public async Task<List<RelatedBlogDto>> GetRelatedBlogsAsync(
    int currentBlogId,
    string? category,
    int take = 3)
        {
            return await _context.Blogs
                .AsNoTracking()
                .Where(x =>
                    x.Id != currentBlogId &&
                    x.Category == category &&
                    x.Status == BlogStatus.Published &&
                    !x.IsDeleted &&
                    x.IsActive)
                .OrderByDescending(x => x.PublishingDate)
                .Take(take)
                .Select(x => new RelatedBlogDto
                {
                    Id = x.Id,

                    Title = x.Title,

                    Slug = x.Slug,

                    CoverImage = x.CoverImage,

                    PublishingDate = x.PublishingDate,

                    ReadTime = x.ReadTime
                })
                .ToListAsync();

        }
        public async Task<List<BlogCardDto>> GetBlogCardsAsync()
        {
            return await
            (
                from blog in _context.Blogs.AsNoTracking()

                join user in _context.Users
                    on blog.CreatedByUserId equals user.ID

                join employee in _context.TblEmployees
                    on user.EmpId equals employee.ID

                where blog.Status == BlogStatus.Published
                      && !blog.IsDeleted
                      && blog.IsActive

                orderby blog.PublishingDate descending

                select new BlogCardDto
                {
                    Id = blog.Id,

                    Title = blog.Title,

                    Slug = blog.Slug,

                    CoverImage = blog.CoverImage,

                    ShortDescription = blog.ShortDescription,

                    PublishingDate = blog.PublishingDate,

                    ReadTime = blog.ReadTime,

                    Author = new BlogCardAuthorDto
                    {
                        Name = employee.FULL_NAME,

                        Designation = employee.Position,

                        ProfileImage =
                            !string.IsNullOrWhiteSpace(employee.Photo)
                                ? employee.Photo
                                : employee.ProfilePicName
                    }
                }

            ).ToListAsync();
        }

        public async Task<Blog?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Blog.Cards;
using RedBerryCorporate.DTOs.Blog.FAQ;
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
                .Include(x => x.CategoryNavigation)
                .Include(x => x.FAQs)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted &&
                    x.IsActive);
        }

        public async Task<Blog?> GetBySlugAsync(string slug)
        {
            return await _context.Blogs
                .Include(x => x.CategoryNavigation)
                .FirstOrDefaultAsync(x =>
                    x.Slug == slug &&
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.Status == BlogStatus.Published);
        }




        public async Task<(List<BlogResponseDto> Blogs, int TotalCount)>
            GetAllAsync(BlogQueryDto query)
        {
            var blogs =
                from blog in _context.Blogs

                join user in _context.Users
                    on blog.CreatedByUserId equals user.ID

                join employee in _context.TblEmployees
                    on user.EmpId equals employee.ID

                where !blog.IsDeleted &&
                      blog.IsActive

                select new
                {
                    Blog = blog,
                    User = user,
                    Employee = employee
                };

            //------------------------------------
            // Search
            //------------------------------------

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                blogs = blogs.Where(x =>
                    x.Blog.Title.Contains(search));
            }

            //------------------------------------
            // Category Filter
            //------------------------------------

            if (query.CategoryId.HasValue)
            {
                blogs = blogs.Where(x =>
                    x.Blog.CategoryId == query.CategoryId.Value);
            }

            //------------------------------------
            // Status Filter
            //------------------------------------

            if (query.Status.HasValue)
            {
                blogs = blogs.Where(x =>
                    x.Blog.Status == query.Status.Value);
            }

            //------------------------------------
            // Sorting
            //------------------------------------

            blogs = query.SortBy?.ToLower() == "oldest"
                ? blogs.OrderBy(x => x.Blog.CreatedAt)
                : blogs.OrderByDescending(x => x.Blog.CreatedAt);

            //------------------------------------
            // Total Count
            //------------------------------------

            var total = await blogs.CountAsync();

            //------------------------------------
            // Pagination
            //------------------------------------

            var data = await blogs
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new BlogResponseDto
                {
                    Id = x.Blog.Id,

                    Title = x.Blog.Title,

                    //------------------------------------
                    // Category
                    //------------------------------------

                    Category = x.Blog.CategoryNavigation == null
                        ? null
                        : new BlogCategoryInfoDto
                        {
                            Id = x.Blog.CategoryNavigation.Id,

                            Name = x.Blog.CategoryNavigation.Name,

                            Slug = x.Blog.CategoryNavigation.Slug
                        },

                    MetaDescription = x.Blog.MetaDescription,

                    ShortDescription = x.Blog.ShortDescription,

                    Slug = x.Blog.Slug,

                    CoverImage = x.Blog.CoverImage,

                    BlogDetails = x.Blog.BlogDetails,

                    Tags = x.Blog.Tags,

                    Status = x.Blog.Status.ToString(),

                    EntryDate = x.Blog.CreatedAt,

                    PublishingDate = x.Blog.PublishingDate,

                    ReadTime = x.Blog.ReadTime,

                    OpenCount = x.Blog.OpenCount,

                    //------------------------------------
                    // Author
                    //------------------------------------

                    Author = new BlogCardAuthorDto
                    {
                        Name = x.Employee.FULL_NAME,

                        Designation = x.Employee.Position,

                        ProfileImage =
                            !string.IsNullOrWhiteSpace(x.Employee.Photo)
                                ? x.Employee.Photo
                                : x.Employee.ProfilePicName
                    }
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<List<Blog>> GetPublishedAsync()
        {
            return await _context.Blogs
                .Include(x => x.CategoryNavigation)
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

        public async Task<BlogViewerResponseDto?> GetViewerAsync(
      string slug)
        {
            var data =
                await
                (
                    from blog in _context.Blogs
                        .AsNoTracking()

                    join user in _context.Users
                        on blog.CreatedByUserId equals user.ID

                    join employee in _context.TblEmployees
                        on user.EmpId equals employee.ID

                    where blog.Slug == slug
                          && blog.Status == BlogStatus.Published
                          && !blog.IsDeleted
                          && blog.IsActive

                    select new BlogViewerResponseDto
                    {
                        Id = blog.Id,

                        Title = blog.Title,

                        Slug = blog.Slug,

                        //------------------------------------
                        // Category
                        //------------------------------------

                        Category = blog.CategoryNavigation == null
                            ? null
                            : new BlogCategoryInfoDto
                            {
                                Id = blog.CategoryNavigation.Id,

                                Name = blog.CategoryNavigation.Name,

                                Slug = blog.CategoryNavigation.Slug
                            },

                        CoverImage = blog.CoverImage,

                        MetaDescription = blog.MetaDescription,

                        BlogDetails = blog.BlogDetails,

                        Tags = blog.Tags,

                        ReadTime = blog.ReadTime,

                        OpenCount = blog.OpenCount,

                        PublishingDate = blog.PublishingDate,

                        //------------------------------------
                        // Author
                        //------------------------------------

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
                        },

                        //--------------------------------
                        // FAQs  will be populated separately
                        //--------------------------------
                        FAQs = blog.FAQs
    .Where(f => f.IsActive)
    .OrderBy(f => f.SortOrder)
    .Select(f => new BlogFaqDto
    {
        Id = f.Id,
        Question = f.Question,
        Answer = f.Answer,
        SortOrder = f.SortOrder,
        IsActive = f.IsActive
    })
    .ToList(),
                    }
                )
                .FirstOrDefaultAsync();

            return data;
        }

        public async Task<List<RelatedBlogDto>> GetRelatedBlogsAsync(
      int currentBlogId,
      int? categoryId,
      int take = 3)
        {
            return await _context.Blogs
                .AsNoTracking()
                .Where(x =>
                    x.Id != currentBlogId &&
                    x.CategoryId == categoryId &&
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
        //public async Task<List<BlogCardDto>> GetBlogCardsAsync()
        //{
        //    return await
        //    (
        //        from blog in _context.Blogs.AsNoTracking()

        //        join user in _context.Users
        //            on blog.CreatedByUserId equals user.ID

        //        join employee in _context.TblEmployees
        //            on user.EmpId equals employee.ID

        //        where blog.Status == BlogStatus.Published
        //              && !blog.IsDeleted
        //              && blog.IsActive

        //        orderby blog.PublishingDate descending

        //        select new BlogCardDto
        //        {
        //            Id = blog.Id,

        //            Title = blog.Title,

        //            Slug = blog.Slug,

        //            CoverImage = blog.CoverImage,

        //            ShortDescription = blog.ShortDescription,

        //            PublishingDate = blog.PublishingDate,

        //            ReadTime = blog.ReadTime,

        //            Author = new BlogCardAuthorDto
        //            {
        //                Name = employee.FULL_NAME,

        //                Designation = employee.Position,

        //                ProfileImage =
        //                    !string.IsNullOrWhiteSpace(employee.Photo)
        //                        ? employee.Photo
        //                        : employee.ProfilePicName
        //            }
        //        }

        //    ).ToListAsync();
        //}
        public async Task<List<BlogCardDto>> GetBlogCardsAsync(
      string? categorySlug = null)
        {
            var query =
                from blog in _context.Blogs.AsNoTracking()

                join user in _context.Users
                    on blog.CreatedByUserId equals user.ID

                join employee in _context.TblEmployees
                    on user.EmpId equals employee.ID

                where blog.Status == BlogStatus.Published
                      && !blog.IsDeleted
                      && blog.IsActive

                select new
                {
                    Blog = blog,
                    Employee = employee
                };

            //---------------------------------------
            // Category Filter
            //---------------------------------------

            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                var normalizedCategorySlug =
                    categorySlug.Trim().ToLower();

                query = query.Where(x =>
                    x.Blog.CategoryNavigation != null &&
                    x.Blog.CategoryNavigation.Slug.ToLower()
                        == normalizedCategorySlug);
            }

            //---------------------------------------
            // Sorting
            //---------------------------------------

            query = query.OrderByDescending(
                x => x.Blog.PublishingDate);

            //---------------------------------------
            // Projection
            //---------------------------------------

            return await query
                .Select(x => new BlogCardDto
                {
                    Id = x.Blog.Id,

                    Title = x.Blog.Title,

                    Slug = x.Blog.Slug,

                    CoverImage = x.Blog.CoverImage,

                    ShortDescription = x.Blog.ShortDescription,

                    PublishingDate = x.Blog.PublishingDate,

                    ReadTime = x.Blog.ReadTime,

                    Author = new BlogCardAuthorDto
                    {
                        Name = x.Employee.FULL_NAME,

                        Designation = x.Employee.Position,

                        ProfileImage =
                            !string.IsNullOrWhiteSpace(x.Employee.Photo)
                                ? x.Employee.Photo
                                : x.Employee.ProfilePicName
                    }
                })
                .ToListAsync();
        }
        public async Task<Blog?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task ReplaceFaqsAsync(
    int blogId,
    List<BlogFaq> faqs)
        {
            var existingFaqs = await _context.BlogFaqs
                .Where(x => x.BlogId == blogId)
                .ToListAsync();

            if (existingFaqs.Any())
            {
                _context.BlogFaqs.RemoveRange(existingFaqs);
            }

            if (faqs.Any())
            {
                await _context.BlogFaqs.AddRangeAsync(faqs);
            }

            await _context.SaveChangesAsync();
        }
    }
}
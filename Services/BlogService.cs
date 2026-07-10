using Microsoft.AspNetCore.Hosting;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Interfaces.Sitemap;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ISitemapGenerator _sitemap;

        public BlogService(
            IBlogRepository repository,
            IWebHostEnvironment environment,ISitemapGenerator sitemap)
        {
            _repository = repository;
            _environment = environment;
            _sitemap = sitemap;
        }

        public async Task<BlogResponseDto> AddAsync(CreateBlogDto dto)
        {
            string slug;

            if (!string.IsNullOrWhiteSpace(dto.Slug))
            {
                slug = dto.Slug.Trim()
                               .Replace(" ", "-")
                               .ToLower();
            }
            else
            {
                slug = dto.Title.Trim()
                                .Replace(" ", "-")
                                .ToLower();
            }

            if (await _repository.SlugExistsAsync(slug))
            {
                throw new Exception("Slug already exists.");
            }

            var imagePath = await ImageHelper.UploadBlogImageAsync(
                dto.CoverImage,
                _environment);

            var blog = new Blog
            {
                Title = dto.Title,
                Category = dto.Category,
                MetaDescription = dto.MetaDescription,
                BlogDetails = dto.BlogDetails,
                Tags = dto.Tags,

                Slug = slug,

                CoverImage = imagePath,

                EntryDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,

                IsActive = true,

                Status = BlogStatus.Draft
            };

            blog = await _repository.AddAsync(blog);
            if (blog.Status == BlogStatus.Published)
            {
                await _sitemap.GenerateAsync();
            }
            return MapToDto(blog);
        }

        public async Task<BlogResponseDto?> UpdateAsync(UpdateBlogDto dto)
        {
            var blog = await _repository.GetByIdAsync(dto.Id);

            if (blog == null)
                return null;

            blog.Title = dto.Title;
            blog.Category = dto.Category;
            blog.MetaDescription = dto.MetaDescription;
            blog.BlogDetails = dto.BlogDetails;
            blog.Tags = dto.Tags;

            if (!string.IsNullOrWhiteSpace(dto.Slug))
            {
                blog.Slug = string.IsNullOrWhiteSpace(dto.Slug)
    ? SlugHelper.Generate(dto.Title)
    : SlugHelper.Generate(dto.Slug);
            }

            if (await _repository.SlugExistsAsync(blog.Slug, blog.Id))
            {
                throw new Exception("Slug already exists.");
            }

            if (dto.CoverImage != null)
            {
                ImageHelper.DeleteImage(
    blog.CoverImage,
    _environment);

                blog.CoverImage = await ImageHelper.UploadBlogImageAsync(
                    dto.CoverImage,
                    _environment);
            }

            blog.UpdateDate = DateTime.UtcNow;

            blog = await _repository.UpdateAsync(blog);
            if (blog.Status == BlogStatus.Published)
            {
                await _sitemap.GenerateAsync();
            }
            return MapToDto(blog);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                await _sitemap.GenerateAsync();
            }

            return result;
        }

        //public async Task<List<BlogResponseDto>> GetAllAsync()
        //{
        //    var blogs = await _repository.GetAllAsync();

        //    return blogs.Select(MapToDto).ToList();
        //}

        public async Task<PagedResponse<BlogResponseDto>> GetAllAsync(BlogQueryDto query)
        {
            var result = await _repository.GetAllAsync(query);

            return new PagedResponse<BlogResponseDto>
            {
                Data = result.Blogs.Select(MapToDto).ToList(),

                PageNumber = query.PageNumber,

                PageSize = query.PageSize,

                TotalRecords = result.TotalCount,

                TotalPages =
                    (int)Math.Ceiling(
                        result.TotalCount /
                        (double)query.PageSize)
            };
        }
        public async Task<List<BlogResponseDto>> GetPublishedAsync()
        {
            var blogs = await _repository.GetPublishedAsync();

            return blogs.Select(MapToDto).ToList();
        }

        public async Task<BlogResponseDto?> GetByIdAsync(int id)
        {
            var blog = await _repository.GetByIdAsync(id);

            if (blog == null)
                return null;

            return MapToDto(blog);
        }

        public async Task<BlogResponseDto?> GetBySlugAsync(string slug)
        {
            var blog = await _repository.GetBySlugAsync(slug);

            if (blog == null)
                return null;

            return MapToDto(blog);
        }

        public async Task<bool> IncrementOpenCountAsync(int id)
        {
            return await _repository.IncrementOpenCountAsync(id);
        }

        private static BlogResponseDto MapToDto(Blog blog)
        {
            return new BlogResponseDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Category = blog.Category,
                MetaDescription = blog.MetaDescription,
                Slug = blog.Slug,
                CoverImage = blog.CoverImage,
                BlogDetails = blog.BlogDetails,
                Tags = blog.Tags,
                Status = blog.Status.ToString(),
                EntryDate = blog.EntryDate,
                PublishingDate = blog.PublishingDate
            };
        }

        public static class SlugHelper
        {
            public static string Generate(string text)
            {
                return text.Trim()
                           .ToLower()
                           .Replace(" ", "-");
            }
        }
    }
}
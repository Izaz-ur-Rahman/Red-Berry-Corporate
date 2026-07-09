using Microsoft.AspNetCore.Hosting;
using RedBerryCorporate.DTOs.Blog;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public BlogService(
            IBlogRepository repository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
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

            if (dto.CoverImage != null)
            {
                blog.CoverImage = await ImageHelper.UploadBlogImageAsync(
                    dto.CoverImage,
                    _environment);
            }

            blog.UpdateDate = DateTime.UtcNow;

            blog = await _repository.UpdateAsync(blog);

            return MapToDto(blog);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<List<BlogResponseDto>> GetAllAsync()
        {
            var blogs = await _repository.GetAllAsync();

            return blogs.Select(MapToDto).ToList();
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
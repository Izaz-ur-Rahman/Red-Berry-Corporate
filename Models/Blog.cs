using System.ComponentModel.DataAnnotations;
using RedBerryCorporate.Enums;

namespace RedBerryCorporate.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        [Required]
        [MaxLength(250)]
        public string Slug { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? MetaDescription { get; set; }

        public string? CoverImage { get; set; }

        public string? BlogDetails { get; set; }

        public string? Tags { get; set; }

        //--------------------------------------------------
        // Blog Workflow
        //--------------------------------------------------

        public BlogStatus Status { get; set; } = BlogStatus.Draft;

        public DateTime? PublishingDate { get; set; }

        public int OpenCount { get; set; }

        public int ReadTime { get; set; }


        //--------------------------------------------------
        // Audit Fields
        //--------------------------------------------------

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedByUserId { get; set; }

        public DateTime? PublishedAt { get; set; }

        public int? PublishedByUserId { get; set; }
        //--------------------------------------------------
        // Soft Delete
        //--------------------------------------------------

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public int? DeletedByUserId { get; set; }
        public bool IsActive { get; set; } = true;

    

    
    }
}
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
        public string Category { get; set; }

        [MaxLength(500)]
        public string MetaDescription { get; set; }

        public string CoverImage { get; set; }

        public string BlogDetails { get; set; }

        public string Tags { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public DateTime? PublishingDate { get; set; }

        public string CreatedBy { get; set; }

        public BlogStatus Status { get; set; } = BlogStatus.Draft;

        public bool IsActive { get; set; } = true;

        public int ReadTime { get; set; }

        public int OpenCount { get; set; }
    }
}
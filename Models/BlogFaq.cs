using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.Models
{
    public class BlogFaq
    {
        public int Id { get; set; }

        public int BlogId { get; set; }

        public Blog Blog { get; set; } = null!;

        [Required]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
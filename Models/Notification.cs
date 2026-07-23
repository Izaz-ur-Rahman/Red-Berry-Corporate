using RedBerryCorporate.Enums;
using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public NotificationType Type { get; set; }
        // Success | Info | Warning | Error

        [Required]
        [MaxLength(50)]
        public NotificationAction Action { get; set; }
        // Created | Updated | Deleted | Published | Archived

        [Required]
        [MaxLength(100)]
        public NotificationModule Module { get; set; }
        // Blog | Contact | Blueprint | EmailTemplate | User

        public int? EntityId { get; set; }

        public bool IsRead { get; set; } = false;

        //--------------------------------------------------
        // Audit Fields
        //--------------------------------------------------

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedByUserId { get; set; }

        //--------------------------------------------------
        // Soft Delete
        //--------------------------------------------------

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public int? DeletedByUserId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
namespace RedBerryCorporate.DTOs.Notification
{
    public class NotificationQueryDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public bool? IsRead { get; set; }

        public string? Module { get; set; }
    }
}
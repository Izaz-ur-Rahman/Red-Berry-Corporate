using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.DTOs.Notification;
using RedBerryCorporate.Enums;

namespace RedBerryCorporate.Interfaces.Notification
{
    public interface INotificationService
    {
        Task CreateAsync(
            string title,
            string message,
            NotificationType type,
            NotificationAction action,
            NotificationModule module,
            int? entityId,
            int? currentUserId);

        Task<PagedResponse<NotificationResponseDto>>
            GetAllAsync(NotificationQueryDto query);

        Task<NotificationResponseDto?>
            GetByIdAsync(int id);

        Task<int> GetUnreadCountAsync();

        Task<bool> MarkAsReadAsync(int id);

        Task<int> MarkAllAsReadAsync();
    }
}
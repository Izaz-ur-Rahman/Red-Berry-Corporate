using RedBerryCorporate.DTOs.Notification;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces.Notification
{
    using NotificationEntity = RedBerryCorporate.Models.Notification;
    public interface INotificationRepository
    {
        Task<NotificationEntity> AddAsync(NotificationEntity notification);

        Task<(List<NotificationEntity> Notifications, int TotalCount)>
            GetAllAsync(NotificationQueryDto query);

        Task<NotificationEntity?> GetByIdAsync(int id);

        Task<int> GetUnreadCountAsync();

        Task<bool> MarkAsReadAsync(int id);

        Task<int> MarkAllAsReadAsync();
    }
}
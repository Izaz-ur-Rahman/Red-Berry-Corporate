using RedBerryCorporate.DTOs.Notification;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces.Notification
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification notification);

        Task<(List<Notification> Notifications, int TotalCount)>
            GetAllAsync(NotificationQueryDto query);

        Task<Notification?> GetByIdAsync(int id);

        Task<int> GetUnreadCountAsync();

        Task<bool> MarkAsReadAsync(int id);

        Task<int> MarkAllAsReadAsync();
    }
}
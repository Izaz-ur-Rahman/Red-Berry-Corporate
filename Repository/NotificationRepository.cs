using Microsoft.EntityFrameworkCore;
using RedBerryCorporate.Data;
using RedBerryCorporate.DTOs.Notification;
using RedBerryCorporate.Interfaces.Notification;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Repositories
{
    public class NotificationRepository
        : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> AddAsync(
            Notification notification)
        {
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<(List<Notification>, int)>
            GetAllAsync(NotificationQueryDto query)
        {
            var notifications =
                _context.Notifications
                    .Where(x => !x.IsDeleted);

            //-----------------------------------
            // Filter by Read
            //-----------------------------------

            if (query.IsRead.HasValue)
            {
                notifications =
                    notifications.Where(x =>
                        x.IsRead == query.IsRead.Value);
            }
            //-----------------------------------
            // Filter by Module
            //-----------------------------------

            if (query.Module.HasValue)
            {
                notifications = notifications.Where(x =>
                    x.Module == query.Module.Value);
            }

            //-----------------------------------
            // Total Count
            //-----------------------------------

            var totalCount =
                await notifications.CountAsync();

            //-----------------------------------
            // Pagination
            //-----------------------------------

            var result =
                await notifications

                .OrderByDescending(x => x.CreatedAt)

                .Skip(
                    (query.PageNumber - 1)
                    * query.PageSize)

                .Take(query.PageSize)

                .ToListAsync();

            return (result, totalCount);
        }

        public async Task<Notification?> GetByIdAsync(
            int id)
        {
            return await _context.Notifications

                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.Notifications

                .CountAsync(x =>
                    !x.IsDeleted &&
                    !x.IsRead);
        }

        public async Task<bool> MarkAsReadAsync(
            int id)
        {
            var notification =
                await GetByIdAsync(id);

            if (notification == null)
                return false;

            notification.IsRead = true;

            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> MarkAllAsReadAsync()
        {
            var notifications =
                await _context.Notifications

                .Where(x =>
                    !x.IsDeleted &&
                    !x.IsRead)

                .ToListAsync();

            foreach (var item in notifications)
            {
                item.IsRead = true;

                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return notifications.Count;
        }
    }
}
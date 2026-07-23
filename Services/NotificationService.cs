using RedBerryCorporate.DTOs.Common;
using RedBerryCorporate.DTOs.Notification;
using RedBerryCorporate.Enums;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces.Notification;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(
            INotificationRepository repository)
        {
            _repository = repository;
        }

        //--------------------------------------------------
        // Create Notification
        //--------------------------------------------------

        public async Task CreateAsync(
            string title,
            string message,
            NotificationType type,
            NotificationAction action,
            NotificationModule module,
            int? entityId,
            int? currentUserId)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,

                Type = type,
                Action = action,
                Module = module,

                EntityId = entityId,

                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,

                IsActive = true,
                IsDeleted = false,
                IsRead = false
            };

            await _repository.AddAsync(notification);
        }

        //--------------------------------------------------
        // Get All
        //--------------------------------------------------

        public async Task<PagedResponse<NotificationResponseDto>>
            GetAllAsync(NotificationQueryDto query)
        {
            var result =
                await _repository.GetAllAsync(query);

            return new PagedResponse<NotificationResponseDto>
            {
                Data = result.Item1
                    .Select(MapToDto)
                    .ToList(),

                PageNumber = query.PageNumber,

                PageSize = query.PageSize,

                TotalRecords = result.Item2,

                TotalPages =
                    (int)Math.Ceiling(
                        result.Item2 /
                        (double)query.PageSize)
            };
        }

        //--------------------------------------------------
        // Get By Id
        //--------------------------------------------------

        public async Task<NotificationResponseDto?>
            GetByIdAsync(int id)
        {
            var notification =
                await _repository.GetByIdAsync(id);

            if (notification == null)
                return null;

            return MapToDto(notification);
        }

        //--------------------------------------------------
        // Unread Count
        //--------------------------------------------------

        public async Task<int> GetUnreadCountAsync()
        {
            return await _repository.GetUnreadCountAsync();
        }

        //--------------------------------------------------
        // Mark Read
        //--------------------------------------------------

        public async Task<bool> MarkAsReadAsync(int id)
        {
            return await _repository.MarkAsReadAsync(id);
        }

        //--------------------------------------------------
        // Mark All Read
        //--------------------------------------------------

        public async Task<int> MarkAllAsReadAsync()
        {
            return await _repository.MarkAllAsReadAsync();
        }

        //--------------------------------------------------
        // Mapping
        //--------------------------------------------------

        private static NotificationResponseDto
            MapToDto(Notification notification)
        {
            return new NotificationResponseDto
            {
                Id = notification.Id,

                Title = notification.Title,

                Message = notification.Message,

                Type = notification.Type.ToString(),

                Action = notification.Action.ToString(),

                Module = notification.Module.ToString(),

                EntityId = notification.EntityId,

                IsRead = notification.IsRead,

                CreatedAt = notification.CreatedAt
            };
        }
    }
}
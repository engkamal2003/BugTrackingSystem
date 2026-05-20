using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using System;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly BugTrackingDbContext _context;

        public NotificationsService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetUserNotifications(int userId)
        {
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    BugId = n.BugId,
                    BugTitle = n.Bug.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();

            return ServiceResult.Ok(notifications);
        }

        public ServiceResult MarkAsRead(int notificationId, int currentUserId)
        {
            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == notificationId && n.UserId == currentUserId && !n.IsDeleted);

            if (notification == null)
                return ServiceResult.Fail("Notification not found.", ServiceStatus.NotFound);

            notification.IsRead = true;
            notification.UpdatedBy = currentUserId;
            notification.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return ServiceResult.Ok(message: "Notification marked as read successfully.");
        }
    }
}

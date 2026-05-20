using BugTrackingSystem.Services;

namespace BugTrackingSystem.Interfaces
{
    public interface INotificationsService
    {
        ServiceResult GetUserNotifications(int userId);
        ServiceResult MarkAsRead(int notificationId, int currentUserId);
    }
}

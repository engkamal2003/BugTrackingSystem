using System;

namespace BugTrackingSystem.DTOs.Responses
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public string BugTitle { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System;

namespace BugTrackingSystem.DTOs.Responses
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public string FileName { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}

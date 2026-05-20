using System;

namespace BugTrackingSystem.DTOs.Responses
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedByName { get; set; }
    }
}

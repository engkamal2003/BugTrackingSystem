using System;

namespace BugTrackingSystem.DTOs.Responses
{
    public class BugDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}

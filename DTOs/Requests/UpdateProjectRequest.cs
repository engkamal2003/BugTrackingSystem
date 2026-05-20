using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class UpdateProjectRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(30)]
        public string Status { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class CreateBugRequest
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; }

        [Required]
        [MaxLength(20)]
        public string Severity { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public List<int> AssignedTo { get; set; }
    }
}

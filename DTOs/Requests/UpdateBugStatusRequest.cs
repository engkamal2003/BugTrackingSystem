using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class UpdateBugStatusRequest
    {
        [Required]
        [MaxLength(30)]
        public string NewStatus { get; set; }
    }
}

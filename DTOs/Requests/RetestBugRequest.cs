using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class RetestBugRequest
    {
        [Required]
        public bool IsFixed { get; set; }

        [MaxLength(1000)]
        public string TesterComment { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class UpdateUserStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}

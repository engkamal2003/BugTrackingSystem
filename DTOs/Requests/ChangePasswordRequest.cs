using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}

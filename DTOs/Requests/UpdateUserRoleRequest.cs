using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class UpdateUserRoleRequest
    {
        [Required]
        public int RoleId { get; set; }
    }
}

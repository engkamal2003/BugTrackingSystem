using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Requests
{
    public class UpdateRolePermissionsRequest
    {
        [Required]
        public List<int> PermissionIds { get; set; }
    }
}

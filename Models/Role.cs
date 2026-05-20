using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
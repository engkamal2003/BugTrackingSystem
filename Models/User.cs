using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        public int RoleId { get; set; }

        public bool MustChangePassword { get; set; } = true;

        public DateTime? PasswordChangedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public virtual Role Role { get; set; }

        // Projects
        public virtual ICollection<Project> CreatedProjects { get; set; }

        public virtual ICollection<Project> UpdatedProjects { get; set; }

        public virtual ICollection<Project> DeletedProjects { get; set; }

        // Bugs
        public virtual ICollection<Bug> CreatedBugs { get; set; }

        public virtual ICollection<Bug> UpdatedBugs { get; set; }

        public virtual ICollection<Bug> DeletedBugs { get; set; }

        public virtual ICollection<Bug> AssignedBugs { get; set; }

        // Comments
        public virtual ICollection<BugComment> Comments { get; set; }

        // Notifications
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
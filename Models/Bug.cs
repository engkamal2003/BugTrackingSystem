using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class Bug : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        [Required]
        [MaxLength(20)]
        public string Severity { get; set; } = "Minor";

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "New";

        public int ProjectId { get; set; }

        public int? AssignedTo { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public virtual Project Project { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual User DeletedByUser { get; set; }

        public virtual User AssignedToUser { get; set; }

        public virtual ICollection<BugComment> Comments { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<BugStatusHistory> StatusHistory { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}

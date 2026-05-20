using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class Project : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Active";

        public virtual User CreatedByUser { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual User DeletedByUser { get; set; }

        public virtual ICollection<Bug> Bugs { get; set; }
    }
}
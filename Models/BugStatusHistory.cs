using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class BugStatusHistory
    {
        public int Id { get; set; }

        public int BugId { get; set; }

        [Required]
        [MaxLength(30)]
        public string OldStatus { get; set; }

        [Required]
        [MaxLength(30)]
        public string NewStatus { get; set; }

        public int ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.Now;

        public virtual Bug Bug { get; set; }

        public virtual User ChangedByUser { get; set; }
    }
}
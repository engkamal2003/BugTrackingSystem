using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class Attachment : BaseEntity
    {
        public int BugId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        public int UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public virtual Bug Bug { get; set; }

        public virtual User UploadedByUser { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual User DeletedByUser { get; set; }
    }
}

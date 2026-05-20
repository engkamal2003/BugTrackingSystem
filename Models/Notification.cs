using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }

        public int BugId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public virtual User User { get; set; }

        public virtual Bug Bug { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual User DeletedByUser { get; set; }
    }
}
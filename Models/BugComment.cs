using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    public class BugComment : BaseEntity
    {
        public int BugId { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string CommentText { get; set; }

        public virtual Bug Bug { get; set; }

        public virtual User User { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User UpdatedByUser { get; set; }

        public virtual User DeletedByUser { get; set; }
    }
}
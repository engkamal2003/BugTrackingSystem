using System;

namespace BugTrackingSystem.Models
{
    public class BugAssignee : BaseEntity
    {
        public int BugId { get; set; }

        public int UserId { get; set; }

        public virtual Bug Bug { get; set; }

        public virtual User User { get; set; }
    }
}

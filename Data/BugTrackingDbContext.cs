using BugTrackingSystem.Models;
using System.Data.Entity;

namespace BugTrackingSystem.Data
{
    public class BugTrackingDbContext : DbContext
    {
        public BugTrackingDbContext() : base("BugTrackingDb")
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<BugAssignee> BugAssignees { get; set; }
        public DbSet<BugComment> BugComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<BugStatusHistory> BugStatusHistories { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → Role
            modelBuilder.Entity<User>()
                .HasRequired(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .WillCascadeOnDelete(false);

            // Project → CreatedByUser
            modelBuilder.Entity<Project>()
                .HasRequired(p => p.CreatedByUser)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(p => p.CreatedBy)
                .WillCascadeOnDelete(false);

            // Project → UpdatedByUser
            modelBuilder.Entity<Project>()
                .HasOptional(p => p.UpdatedByUser)
                .WithMany(u => u.UpdatedProjects)
                .HasForeignKey(p => p.UpdatedBy)
                .WillCascadeOnDelete(false);

            // Project → DeletedByUser
            modelBuilder.Entity<Project>()
                .HasOptional(p => p.DeletedByUser)
                .WithMany(u => u.DeletedProjects)
                .HasForeignKey(p => p.DeletedBy)
                .WillCascadeOnDelete(false);

            // Bug → Project
            modelBuilder.Entity<Bug>()
                .HasRequired(b => b.Project)
                .WithMany(p => p.Bugs)
                .HasForeignKey(b => b.ProjectId)
                .WillCascadeOnDelete(false);

            // Bug → CreatedByUser
            modelBuilder.Entity<Bug>()
                .HasRequired(b => b.CreatedByUser)
                .WithMany(u => u.CreatedBugs)
                .HasForeignKey(b => b.CreatedBy)
                .WillCascadeOnDelete(false);

            // Bug → UpdatedByUser
            modelBuilder.Entity<Bug>()
                .HasOptional(b => b.UpdatedByUser)
                .WithMany(u => u.UpdatedBugs)
                .HasForeignKey(b => b.UpdatedBy)
                .WillCascadeOnDelete(false);

            // Bug → AssignedToUser
            modelBuilder.Entity<Bug>()
                .HasOptional(b => b.AssignedToUser)
                .WithMany(u => u.AssignedBugs)
                .HasForeignKey(b => b.AssignedTo)
                .WillCascadeOnDelete(false);

            // Bug → DeletedByUser
            modelBuilder.Entity<Bug>()
                .HasOptional(b => b.DeletedByUser)
                .WithMany(u => u.DeletedBugs)
                .HasForeignKey(b => b.DeletedBy)
                .WillCascadeOnDelete(false);

            // BugComment → Bug
            modelBuilder.Entity<BugComment>()
                .HasRequired(c => c.Bug)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BugId)
                .WillCascadeOnDelete(false);

            // BugComment → User (author)
            modelBuilder.Entity<BugComment>()
                .HasRequired(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);

            // BugComment → CreatedByUser
            modelBuilder.Entity<BugComment>()
                .HasRequired(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedBy)
                .WillCascadeOnDelete(false);

            // BugComment → UpdatedByUser
            modelBuilder.Entity<BugComment>()
                .HasOptional(c => c.UpdatedByUser)
                .WithMany()
                .HasForeignKey(c => c.UpdatedBy)
                .WillCascadeOnDelete(false);

            // BugComment → DeletedByUser
            modelBuilder.Entity<BugComment>()
                .HasOptional(c => c.DeletedByUser)
                .WithMany()
                .HasForeignKey(c => c.DeletedBy)
                .WillCascadeOnDelete(false);

            // Notification → User (recipient)
            modelBuilder.Entity<Notification>()
                .HasRequired(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .WillCascadeOnDelete(false);

            // Notification → Bug
            modelBuilder.Entity<Notification>()
                .HasRequired(n => n.Bug)
                .WithMany(b => b.Notifications)
                .HasForeignKey(n => n.BugId)
                .WillCascadeOnDelete(false);

            // Notification → CreatedByUser
            modelBuilder.Entity<Notification>()
                .HasRequired(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(n => n.CreatedBy)
                .WillCascadeOnDelete(false);

            // Notification → UpdatedByUser
            modelBuilder.Entity<Notification>()
                .HasOptional(n => n.UpdatedByUser)
                .WithMany()
                .HasForeignKey(n => n.UpdatedBy)
                .WillCascadeOnDelete(false);

            // Notification → DeletedByUser
            modelBuilder.Entity<Notification>()
                .HasOptional(n => n.DeletedByUser)
                .WithMany()
                .HasForeignKey(n => n.DeletedBy)
                .WillCascadeOnDelete(false);

            // BugStatusHistory → Bug
            modelBuilder.Entity<BugStatusHistory>()
                .HasRequired(h => h.Bug)
                .WithMany(b => b.StatusHistory)
                .HasForeignKey(h => h.BugId)
                .WillCascadeOnDelete(false);

            // BugStatusHistory → ChangedByUser
            modelBuilder.Entity<BugStatusHistory>()
                .HasRequired(h => h.ChangedByUser)
                .WithMany()
                .HasForeignKey(h => h.ChangedBy)
                .WillCascadeOnDelete(false);

            // Attachment → Bug
            modelBuilder.Entity<Attachment>()
                .HasRequired(a => a.Bug)
                .WithMany(b => b.Attachments)
                .HasForeignKey(a => a.BugId)
                .WillCascadeOnDelete(false);

            // Attachment → UploadedByUser
            modelBuilder.Entity<Attachment>()
                .HasRequired(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedBy)
                .WillCascadeOnDelete(false);

            // Attachment → CreatedByUser
            modelBuilder.Entity<Attachment>()
                .HasRequired(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .WillCascadeOnDelete(false);

            // Attachment → UpdatedByUser
            modelBuilder.Entity<Attachment>()
                .HasOptional(a => a.UpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.UpdatedBy)
                .WillCascadeOnDelete(false);

            // Attachment → DeletedByUser
            modelBuilder.Entity<Attachment>()
                .HasOptional(a => a.DeletedByUser)
                .WithMany()
                .HasForeignKey(a => a.DeletedBy)
                .WillCascadeOnDelete(false);

            // RolePermission composite key
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasRequired(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RolePermission>()
                .HasRequired(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .WillCascadeOnDelete(false);

            // BugAssignee → Bug
            modelBuilder.Entity<BugAssignee>()
                .HasRequired(ba => ba.Bug)
                .WithMany(b => b.Assignees)
                .HasForeignKey(ba => ba.BugId)
                .WillCascadeOnDelete(true);

            // BugAssignee → User (Assignee)
            modelBuilder.Entity<BugAssignee>()
                .HasRequired(ba => ba.User)
                .WithMany()
                .HasForeignKey(ba => ba.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}

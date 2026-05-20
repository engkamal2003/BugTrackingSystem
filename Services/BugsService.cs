using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class BugsService : IBugsService
    {
        private readonly BugTrackingDbContext _context;

        public BugsService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetBugs()
        {
            var bugs = _context.Bugs
                .Where(b => !b.IsDeleted)
                .Select(b => new BugDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Priority = b.Priority,
                    Severity = b.Severity,
                    Status = b.Status,
                    ProjectId = b.ProjectId,
                    ProjectName = b.Project.Name,
                    AssignedTo = b.AssignedTo,
                    AssignedToName = b.AssignedToUser != null ? b.AssignedToUser.FullName : null,
                    CreatedAt = b.CreatedAt,
                    CreatedByName = b.CreatedByUser.FullName,
                    UpdatedAt = b.UpdatedAt,
                    ResolvedAt = b.ResolvedAt
                })
                .ToList();

            return ServiceResult.Ok(bugs);
        }

        public ServiceResult CreateBug(CreateBugRequest request, int currentUserId)
        {
            if (!_context.Projects.Any(p => p.Id == request.ProjectId && !p.IsDeleted))
                return ServiceResult.Fail("Project does not exist.");

            var developer = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == request.AssignedTo && !u.IsDeleted && u.Role.Name == "Developer");

            if (developer == null)
                return ServiceResult.Fail("Assigned user must be an active Developer.");

            var bug = new Bug
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Severity = request.Severity,
                Status = "New",
                ProjectId = request.ProjectId,
                AssignedTo = request.AssignedTo,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            };

            _context.Bugs.Add(bug);
            _context.SaveChanges();

            _context.Notifications.Add(new Notification
            {
                UserId = request.AssignedTo,
                BugId = bug.Id,
                Message = "You have been assigned a new bug.",
                IsRead = false,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            });

            _context.BugStatusHistories.Add(new BugStatusHistory
            {
                BugId = bug.Id,
                OldStatus = "None",
                NewStatus = "New",
                ChangedBy = currentUserId,
                ChangedAt = DateTime.Now
            });

            _context.SaveChanges();

            return ServiceResult.Ok(new { BugId = bug.Id }, "Bug created successfully.");
        }

        public ServiceResult UpdateBugStatus(int id, UpdateBugStatusRequest request, int currentUserId, string currentUserRole)
        {
            var bug = _context.Bugs.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bug == null)
                return ServiceResult.Fail("Bug not found.", ServiceStatus.NotFound);

            if (currentUserRole == "Developer" && bug.AssignedTo != currentUserId)
                return ServiceResult.Fail("You can only update bugs assigned to you.", ServiceStatus.Forbidden);

            var oldStatus = bug.Status;

            bug.Status = request.NewStatus;
            bug.UpdatedBy = currentUserId;
            bug.UpdatedAt = DateTime.Now;

            if (request.NewStatus == "Resolved")
                bug.ResolvedAt = DateTime.Now;

            _context.BugStatusHistories.Add(new BugStatusHistory
            {
                BugId = bug.Id,
                OldStatus = oldStatus,
                NewStatus = request.NewStatus,
                ChangedBy = currentUserId,
                ChangedAt = DateTime.Now
            });

            _context.Notifications.Add(new Notification
            {
                UserId = bug.CreatedBy,
                BugId = bug.Id,
                Message = "Bug status changed from " + oldStatus + " to " + request.NewStatus + ".",
                IsRead = false,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return ServiceResult.Ok(new { BugId = bug.Id, OldStatus = oldStatus, NewStatus = request.NewStatus }, "Bug status updated successfully.");
        }

        public ServiceResult RetestBug(int id, RetestBugRequest request, int currentUserId)
        {
            var bug = _context.Bugs.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bug == null)
                return ServiceResult.Fail("Bug not found.", ServiceStatus.NotFound);

            if (bug.Status != "Resolved")
                return ServiceResult.Fail("Only resolved bugs can be retested.");

            var oldStatus = bug.Status;
            var newStatus = request.IsFixed ? "Closed" : "Reopened";

            bug.Status = newStatus;
            bug.UpdatedBy = currentUserId;
            bug.UpdatedAt = DateTime.Now;

            _context.BugStatusHistories.Add(new BugStatusHistory
            {
                BugId = bug.Id,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedBy = currentUserId,
                ChangedAt = DateTime.Now
            });

            if (!string.IsNullOrWhiteSpace(request.TesterComment))
            {
                _context.BugComments.Add(new BugComment
                {
                    BugId = bug.Id,
                    UserId = currentUserId,
                    CommentText = request.TesterComment,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.Now
                });
            }

            if (!request.IsFixed && bug.AssignedTo.HasValue)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = bug.AssignedTo.Value,
                    BugId = bug.Id,
                    Message = "Bug has been reopened by tester. Please check it again.",
                    IsRead = false,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.Now
                });
            }

            _context.SaveChanges();

            return ServiceResult.Ok(
                new { BugId = bug.Id, OldStatus = oldStatus, NewStatus = newStatus },
                request.IsFixed ? "Bug closed successfully after retest." : "Bug reopened and developer notified successfully."
            );
        }
    }
}

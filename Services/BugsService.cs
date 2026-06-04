using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Models;
using System;
using System.Collections.Generic;
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

        public ServiceResult GetBugs(int currentUserId, string currentUserRole)
        {
            var query = _context.Bugs
                .Where(b => !b.IsDeleted)
                .Include(b => b.Assignees);

            // Isolation: Developers يرون فقط أخطاءهم المسندة إليهم
            if (currentUserRole == "Developer")
            {
                var assignedBugIds = _context.BugAssignees
                    .Where(ba => ba.UserId == currentUserId)
                    .Select(ba => ba.BugId)
                    .ToList();

                query = query.Where(b => assignedBugIds.Contains(b.Id));
            }

            var bugs = query
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
                    AssignedToNames = b.Assignees.Select(a => a.User.FullName).ToList(),
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

            if (request.AssignedTo == null || request.AssignedTo.Count == 0)
                return ServiceResult.Fail("Please assign the bug to at least one developer.");

            // تحقق من أن جميع المستخدمين developers نشطاء
            var developers = _context.Users
                .Include(u => u.Role)
                .Where(u => request.AssignedTo.Contains(u.Id) && !u.IsDeleted && u.Role.Name == "Developer")
                .ToList();

            if (developers.Count != request.AssignedTo.Count)
                return ServiceResult.Fail("All assigned users must be active developers.");

            var bug = new Bug
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Severity = request.Severity,
                Status = "New",
                ProjectId = request.ProjectId,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            };

            _context.Bugs.Add(bug);
            _context.SaveChanges();

            // أضف مساندة متعددة
            foreach (var developerId in request.AssignedTo)
            {
                _context.BugAssignees.Add(new BugAssignee
                {
                    BugId = bug.Id,
                    UserId = developerId,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.Now
                });

                // أرسل إشعار لكل developer
                _context.Notifications.Add(new Notification
                {
                    UserId = developerId,
                    BugId = bug.Id,
                    Message = "You have been assigned a new bug.",
                    IsRead = false,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.Now
                });
            }

            _context.BugStatusHistories.Add(new BugStatusHistory
            {
                BugId = bug.Id,
                OldStatus = "None",
                NewStatus = "New",
                ChangedBy = currentUserId,
                ChangedAt = DateTime.Now
            });

            _context.SaveChanges();

            return ServiceResult.Ok(new { BugId = bug.Id }, "Bug created and assigned successfully.");
        }

        public ServiceResult UpdateBugStatus(int id, UpdateBugStatusRequest request, int currentUserId, string currentUserRole)
        {
            var bug = _context.Bugs
                .Include(b => b.Assignees)
                .FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (bug == null)
                return ServiceResult.Fail("Bug not found.", ServiceStatus.NotFound);

            if (currentUserRole == "Developer")
            {
                var isAssigned = bug.Assignees.Any(a => a.UserId == currentUserId);
                if (!isAssigned)
                    return ServiceResult.Fail("You can only update bugs assigned to you.", ServiceStatus.Forbidden);
            }

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
            var bug = _context.Bugs
                .Include(b => b.Assignees)
                .FirstOrDefault(b => b.Id == id && !b.IsDeleted);
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

            if (!request.IsFixed && bug.Assignees.Count > 0)
            {
                foreach (var assignee in bug.Assignees)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = assignee.UserId,
                        BugId = bug.Id,
                        Message = "Bug has been reopened by tester. Please check it again.",
                        IsRead = false,
                        CreatedBy = currentUserId,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();

            return ServiceResult.Ok(
                new { BugId = bug.Id, OldStatus = oldStatus, NewStatus = newStatus },
                request.IsFixed ? "Bug closed successfully after retest." : "Bug reopened and developers notified successfully."
            );
        }
    }
}

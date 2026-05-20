using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using System.Data.Entity;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly BugTrackingDbContext _context;

        public DashboardService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetSummary(int currentUserId)
        {
            var currentUser = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == currentUserId && !u.IsDeleted);

            if (currentUser == null)
                return ServiceResult.Fail("Invalid current user.");

            var userDto = new UserDto
            {
                Id = currentUser.Id,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                RoleId = currentUser.RoleId,
                RoleName = currentUser.Role.Name,
                MustChangePassword = currentUser.MustChangePassword,
                LastLoginAt = currentUser.LastLoginAt,
                CreatedAt = currentUser.CreatedAt,
                IsDeleted = currentUser.IsDeleted
            };

            var summary = new DashboardSummaryDto
            {
                TotalProjects = _context.Projects.Count(p => !p.IsDeleted),
                TotalBugs = _context.Bugs.Count(b => !b.IsDeleted),
                OpenBugs = _context.Bugs.Count(b => !b.IsDeleted && (b.Status == "New" || b.Status == "InProgress" || b.Status == "Reopened")),
                ResolvedBugs = _context.Bugs.Count(b => !b.IsDeleted && b.Status == "Resolved"),
                ClosedBugs = _context.Bugs.Count(b => !b.IsDeleted && b.Status == "Closed"),
                ReopenedBugs = _context.Bugs.Count(b => !b.IsDeleted && b.Status == "Reopened"),
                DevelopersCount = _context.Users.Count(u => !u.IsDeleted && u.Role.Name == "Developer"),
                TestersCount = _context.Users.Count(u => !u.IsDeleted && u.Role.Name == "Tester"),
                UnreadNotifications = _context.Notifications.Count(n => n.UserId == currentUserId && !n.IsRead && !n.IsDeleted)
            };

            return ServiceResult.Ok(new { CurrentUser = userDto, Summary = summary });
        }
    }
}

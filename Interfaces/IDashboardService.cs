using BugTrackingSystem.Services;

namespace BugTrackingSystem.Interfaces
{
    public interface IDashboardService
    {
        ServiceResult GetSummary(int currentUserId);
    }
}
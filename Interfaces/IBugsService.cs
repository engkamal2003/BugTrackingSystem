using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Interfaces
{
    public interface IBugsService
    {
        ServiceResult GetBugs(int currentUserId, string currentUserRole);
        ServiceResult CreateBug(CreateBugRequest request, int currentUserId);
        ServiceResult UpdateBugStatus(int id, UpdateBugStatusRequest request, int currentUserId, string currentUserRole);
        ServiceResult RetestBug(int id, RetestBugRequest request, int currentUserId);
    }
}

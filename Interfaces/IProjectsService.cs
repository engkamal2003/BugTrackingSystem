using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Interfaces
{
    public interface IProjectsService
    {
        ServiceResult GetProjects();
        ServiceResult CreateProject(CreateProjectRequest request, int currentUserId);
        ServiceResult UpdateProject(int id, UpdateProjectRequest request, int currentUserId);
        ServiceResult DeleteProject(int id, int currentUserId);
    }
}

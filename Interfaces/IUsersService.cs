using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Interfaces
{
    public interface IUsersService
    {
        ServiceResult GetUsers();
        ServiceResult CreateUser(CreateUserRequest request, int currentUserId);
        ServiceResult UpdateUserRole(int id, int roleId);
        ServiceResult UpdateUserStatus(int id, bool isActive, int currentUserId);
    }
}

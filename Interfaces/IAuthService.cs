using BugTrackingSystem.Services;

namespace BugTrackingSystem.Interfaces
{
    public interface IAuthService
    {
        ServiceResult Login(string email, string password);
        ServiceResult ChangePassword(int userId, string oldPassword, string newPassword);
    }
}

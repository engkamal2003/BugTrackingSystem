using BugTrackingSystem.Services;
using System.Collections.Generic;

namespace BugTrackingSystem.Interfaces
{
    public interface IPermissionsService
    {
        ServiceResult GetAllPermissions();
        ServiceResult GetRolePermissions(int roleId);
        ServiceResult UpdateRolePermissions(int roleId, List<int> permissionIds);
    }
}

using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly BugTrackingDbContext _context;

        public PermissionsService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetAllPermissions()
        {
            var permissions = _context.Permissions
                .OrderBy(p => p.Group)
                .ThenBy(p => p.Name)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Group = p.Group
                })
                .ToList();

            return ServiceResult.Ok(permissions);
        }

        public ServiceResult GetRolePermissions(int roleId)
        {
            var role = _context.Roles.Find(roleId);
            if (role == null)
                return ServiceResult.Fail("Role not found.", ServiceStatus.NotFound);

            var allPermissions = _context.Permissions
                .OrderBy(p => p.Group)
                .ThenBy(p => p.Name)
                .ToList();

            var grantedIds = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToList();

            var result = allPermissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Group = p.Group,
                IsGranted = grantedIds.Contains(p.Id)
            }).ToList();

            return ServiceResult.Ok(result);
        }

        public ServiceResult UpdateRolePermissions(int roleId, List<int> permissionIds)
        {
            var role = _context.Roles.Find(roleId);
            if (role == null)
                return ServiceResult.Fail("Role not found.", ServiceStatus.NotFound);

            var existing = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToList();

            _context.RolePermissions.RemoveRange(existing);

            foreach (var permId in permissionIds.Distinct())
            {
                var permExists = _context.Permissions.Any(p => p.Id == permId);
                if (permExists)
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permId
                    });
                }
            }

            _context.SaveChanges();

            return ServiceResult.Ok(message: "Role permissions updated successfully.");
        }
    }
}

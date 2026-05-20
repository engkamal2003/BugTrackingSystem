using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class RolesService : IRolesService
    {
        private readonly BugTrackingDbContext _context;

        public RolesService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetRoles()
        {
            var roles = _context.Roles
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();

            return ServiceResult.Ok(roles);
        }
    }
}

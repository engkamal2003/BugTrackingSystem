using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Filters;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/permissions")]
    public class PermissionsController : ApiController
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }

        [RequirePermission("ViewPermissions")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllPermissions()
        {
            var result = _permissionsService.GetAllPermissions();
            return Ok(result.Data);
        }

        [RequirePermission("ViewPermissions")]
        [HttpGet]
        [Route("roles/{roleId:int}")]
        public IHttpActionResult GetRolePermissions(int roleId)
        {
            var result = _permissionsService.GetRolePermissions(roleId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [RequirePermission("ManagePermissions")]
        [HttpPut]
        [Route("roles/{roleId:int}")]
        public IHttpActionResult UpdateRolePermissions(int roleId, UpdateRolePermissionsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _permissionsService.UpdateRolePermissions(roleId, request.PermissionIds);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }
    }
}

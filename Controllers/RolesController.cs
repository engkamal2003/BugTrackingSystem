using BugTrackingSystem.Filters;
using BugTrackingSystem.Interfaces;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetRoles()
        {
            var result = _rolesService.GetRoles();
            return Ok(result.Data);
        }
    }
}

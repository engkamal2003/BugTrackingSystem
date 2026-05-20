using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Filters;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RequirePermission("ViewUsers")]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetUsers()
        {
            var result = _usersService.GetUsers();
            return Ok(result.Data);
        }

        [RequirePermission("CreateUser")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateUser(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _usersService.CreateUser(request, currentUserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { result.Message, Data = result.Data });
        }

        [RequirePermission("UpdateUserRole")]
        [HttpPut]
        [Route("{id:int}/role")]
        public IHttpActionResult UpdateUserRole(int id, UpdateUserRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _usersService.UpdateUserRole(id, request.RoleId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }

        [RequirePermission("UpdateUserStatus")]
        [HttpPut]
        [Route("{id:int}/status")]
        public IHttpActionResult UpdateUserStatus(int id, UpdateUserStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _usersService.UpdateUserStatus(id, request.IsActive, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }
    }
}

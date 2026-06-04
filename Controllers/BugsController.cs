using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Filters;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using System.Net;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/bugs")]
    public class BugsController : ApiController
    {
        private readonly IBugsService _bugsService;

        public BugsController(IBugsService bugsService)
        {
            _bugsService = bugsService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetBugs()
        {
            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var currentUserRole = JwtHelper.GetCurrentUserRole(this);
            var result = _bugsService.GetBugs(currentUserId, currentUserRole);
            return Ok(result.Data);
        }

        [RequirePermission("CreateBug")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateBug(CreateBugRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _bugsService.CreateBug(request, currentUserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { result.Message, Data = result.Data });
        }

        [RequirePermission("UpdateBugStatus")]
        [HttpPut]
        [Route("{id:int}/status")]
        public IHttpActionResult UpdateBugStatus(int id, UpdateBugStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var currentUserRole = JwtHelper.GetCurrentUserRole(this);
            var result = _bugsService.UpdateBugStatus(id, request, currentUserId, currentUserRole);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                if (result.Status == ServiceStatus.Forbidden)
                    return Content(HttpStatusCode.Forbidden, new { Success = false, result.Message });
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message, Data = result.Data });
        }

        [RequirePermission("RetestBug")]
        [HttpPut]
        [Route("{id:int}/retest")]
        public IHttpActionResult RetestBug(int id, RetestBugRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _bugsService.RetestBug(id, request, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message, Data = result.Data });
        }
    }
}

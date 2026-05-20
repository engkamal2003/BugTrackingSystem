using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Filters;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/projects")]
    public class ProjectsController : ApiController
    {
        private readonly IProjectsService _projectsService;

        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetProjects()
        {
            var result = _projectsService.GetProjects();
            return Ok(result.Data);
        }

        [RequirePermission("CreateProject")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateProject(CreateProjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _projectsService.CreateProject(request, currentUserId);

            return Ok(new { result.Message, Data = result.Data });
        }

        [RequirePermission("UpdateProject")]
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateProject(int id, UpdateProjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _projectsService.UpdateProject(id, request, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }

        [RequirePermission("DeleteProject")]
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteProject(int id)
        {
            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _projectsService.DeleteProject(id, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }
    }
}

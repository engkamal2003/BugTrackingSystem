using BugTrackingSystem.Filters;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [RequirePermission("ViewDashboard")]
        [HttpGet]
        [Route("summary")]
        public IHttpActionResult GetSummary()
        {
            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _dashboardService.GetSummary(currentUserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
    }
}

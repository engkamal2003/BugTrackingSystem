using BugTrackingSystem.Filters;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/users")]
    public class NotificationsController : ApiController
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        [RequirePermission("ViewNotifications")]
        [HttpGet]
        [Route("{userId:int}/notifications")]
        public IHttpActionResult GetUserNotifications(int userId)
        {
            var result = _notificationsService.GetUserNotifications(userId);
            return Ok(result.Data);
        }

        [RequirePermission("MarkNotificationRead")]
        [HttpPut]
        [Route("notifications/{notificationId:int}/read")]
        public IHttpActionResult MarkAsRead(int notificationId)
        {
            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _notificationsService.MarkAsRead(notificationId, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }
    }
}

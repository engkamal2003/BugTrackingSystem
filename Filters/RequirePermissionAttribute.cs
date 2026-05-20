using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BugTrackingSystem.Filters
{
    public class RequirePermissionAttribute : AuthorizationFilterAttribute
    {
        private readonly string _permission;

        public RequirePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var identity = actionContext.RequestContext.Principal?.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { success = false, message = "Authentication required." });
                return;
            }

            var hasPermission = identity.Claims
                .Any(c => c.Type == "Permission" && c.Value == _permission);

            if (!hasPermission)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new { success = false, message = $"You don't have the '{_permission}' permission." });
            }
        }
    }
}

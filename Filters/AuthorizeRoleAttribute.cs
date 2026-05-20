using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BugTrackingSystem.Filters
{
    public class AuthorizeRoleAttribute : AuthorizationFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var identity = actionContext.RequestContext.Principal?.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                    {
                        Success = false,
                        Message = "User is not authenticated."
                    });

                return;
            }

            var role = identity.Claims
                .FirstOrDefault(c => c.Type == "Role")
                ?.Value;

            if (string.IsNullOrEmpty(role) || !_allowedRoles.Contains(role))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new
                    {
                        Success = false,
                        Message = "You do not have permission to access this resource."
                    });
            }
        }
    }
}
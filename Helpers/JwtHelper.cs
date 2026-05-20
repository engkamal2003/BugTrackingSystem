using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace BugTrackingSystem.Helpers
{
    public static class JwtHelper
    {
        public static int GetCurrentUserId(ApiController controller)
        {
            var identity = controller.User.Identity as ClaimsIdentity;

            if (identity == null)
                return 0;

            var userIdClaim = identity.Claims
                .FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
                return 0;

            return int.Parse(userIdClaim.Value);
        }

        public static string GetCurrentUserRole(ApiController controller)
        {
            var identity = controller.User.Identity as ClaimsIdentity;

            if (identity == null)
                return null;

            var roleClaim = identity.Claims
                .FirstOrDefault(c => c.Type == "Role");

            return roleClaim?.Value;
        }
    }
}
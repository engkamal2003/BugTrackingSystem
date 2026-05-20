using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.Interfaces;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.Login(request.Email, request.Password);

            if (!result.Success)
                return Ok(new { Success = false, result.Message });

            return Ok(result.Data);
        }

        [HttpPost]
        [Route("change-password")]
        public IHttpActionResult ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.ChangePassword(request.UserId, request.OldPassword, request.NewPassword);

            if (!result.Success)
                return Ok(new { Success = false, result.Message });

            return Ok(new { Success = true, result.Message });
        }
    }
}

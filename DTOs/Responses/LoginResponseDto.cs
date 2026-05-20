namespace BugTrackingSystem.DTOs.Responses
{
    public class LoginResponseDto
    {
        public bool MustChangePassword { get; set; }
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}

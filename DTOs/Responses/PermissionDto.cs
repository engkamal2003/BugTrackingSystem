namespace BugTrackingSystem.DTOs.Responses
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool IsGranted { get; set; }
    }
}

namespace BugTrackingSystem.DTOs.Responses
{
    public class DashboardSummaryDto
    {
        public int TotalProjects { get; set; }
        public int TotalBugs { get; set; }
        public int OpenBugs { get; set; }
        public int ResolvedBugs { get; set; }
        public int ClosedBugs { get; set; }
        public int ReopenedBugs { get; set; }
        public int DevelopersCount { get; set; }
        public int TestersCount { get; set; }
        public int UnreadNotifications { get; set; }
    }
}

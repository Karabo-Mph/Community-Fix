namespace CommunityFix.API.Models
{

    public enum ReportStatus
    {
        Submitted,
        Assigned,
        InProgress,
        Resolved,
        Closed
    }
    public class Report
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Submitted;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

using CommunityFix.API.Models;

namespace CommunityFix.API.DTOs;

public record CreateReportDto(string Title, string Description, int CategoryId, double Latitude, double Longitude);
public record UpdateStatusDto(ReportStatus Status);
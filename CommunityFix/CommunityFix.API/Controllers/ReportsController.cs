using System.Security.Claims;
using CommunityFix.API.Data;
using CommunityFix.API.DTOs;
using CommunityFix.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityFix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db) => _db = db;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    private bool IsAdmin =>
        User.IsInRole(Roles.Administrator) || User.IsInRole(Roles.SystemAdmin);

    [HttpGet]
    public async Task<IActionResult> GetReports()
    {
        var query = _db.Reports.Include(r => r.Category).AsQueryable();

        // Residents see only their own reports. Administrators see everything.
        if (!IsAdmin)
            query = query.Where(r => r.UserId == CurrentUserId);

        var reports = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reports);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReport(int id)
    {
        var report = await _db.Reports
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report is null)
            return NotFound();

        if (!IsAdmin && report.UserId != CurrentUserId)
            return Forbid();

        return Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport(CreateReportDto dto)
    {
        var report = new Report
        {
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            UserId = CurrentUserId,
            Status = ReportStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            ReferenceNumber =
                $"CF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}"
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = $"{Roles.Administrator},{Roles.SystemAdmin}")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
    {
        var report = await _db.Reports.FindAsync(id);
        if (report is null)
            return NotFound();

        report.Status = dto.Status;
        await _db.SaveChangesAsync();

        return Ok(report);
    }
}
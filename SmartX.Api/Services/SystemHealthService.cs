using Microsoft.EntityFrameworkCore;
using SmartX.Api.Hubs;
using SmartX.Api.Infrastructure.Data;

namespace SmartX.Api.Services;

public class SystemHealthService
{
    private readonly AppDbContext _db;
    private readonly ConnectionTracker _tracker;
    private static readonly DateTime StartedAt = DateTime.UtcNow;

    public SystemHealthService(AppDbContext db, ConnectionTracker tracker)
    {
        _db = db;
        _tracker = tracker;
    }

    public async Task<object> GetSummaryAsync()
    {
        var dbConnected = await _db.Database.CanConnectAsync();
        var sensorCount = await _db.Sensors.CountAsync();
        var onlineCount = await _db.Sensors.CountAsync(s => s.Status == "Online");
        var telemetryCount = await _db.TelemetryLogs.CountAsync();
        var criticalLast24h = await _db.AnomalyLogs.CountAsync(a =>
            a.Severity == "Critical" && a.Timestamp > DateTime.UtcNow.AddHours(-24));

        return new
        {
            uptimeSeconds = (DateTime.UtcNow - StartedAt).TotalSeconds,
            dbConnected,
            totalSensors = sensorCount,
            onlineSensors = onlineCount,
            totalTelemetryLogs = telemetryCount,
            criticalAnomaliesLast24h = criticalLast24h,
            liveHubConnections = _tracker.Count
        };
    }

    public Task<List<Domain.Entities.AnomalyLog>> GetRecentIncidentsAsync(int count = 10) =>
        _db.AnomalyLogs.AsNoTracking()
            .Where(a => a.Severity == "Critical")
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
}
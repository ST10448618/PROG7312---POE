using Microsoft.EntityFrameworkCore;
using SmartX.Api.Domain.Entities;
using SmartX.Api.Infrastructure.Data;

namespace SmartX.Api.Services;

public class AnomalyLogService
{
    private readonly AppDbContext _db;
    public AnomalyLogService(AppDbContext db) => _db = db;

    public async Task<List<AnomalyLog>> SearchAsync(string? severity, string? search, int page, int pageSize)
    {
        var query = _db.AnomalyLogs.AsNoTracking().OrderByDescending(a => a.Timestamp).AsQueryable();

        if (!string.IsNullOrWhiteSpace(severity) && severity != "All")
            query = query.Where(a => a.Severity == severity);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.SensorId.Contains(search));

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<AnomalyLog> AcknowledgeAsync(int id)
    {
        var log = await _db.AnomalyLogs.FindAsync(id) ?? throw new KeyNotFoundException("Anomaly log not found.");
        log.Acknowledged = true;
        await _db.SaveChangesAsync();
        return log;
    }
}
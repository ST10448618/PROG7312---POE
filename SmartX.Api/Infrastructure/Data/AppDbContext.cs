using Microsoft.EntityFrameworkCore;
using SmartX.Api.Domain.Entities;

namespace SmartX.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SensorProfile> Sensors => Set<SensorProfile>();
    public DbSet<SensorFile> SensorFiles => Set<SensorFile>();
    public DbSet<TelemetryLog> TelemetryLogs => Set<TelemetryLog>();
}
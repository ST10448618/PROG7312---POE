using Microsoft.EntityFrameworkCore;
using SmartX.Api.Domain.Entities;

namespace SmartX.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SensorProfile> Sensors => Set<SensorProfile>();
    public DbSet<SensorFile> SensorFiles => Set<SensorFile>();
    public DbSet<TelemetryLog> TelemetryLogs => Set<TelemetryLog>();
    public DbSet<AnomalyLog> AnomalyLogs => Set<AnomalyLog>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorProfile>()
            .HasIndex(s => s.MacAddress)
            .IsUnique();

        modelBuilder.Entity<SensorProfile>()
            .HasMany(s => s.Files)
            .WithOne(f => f.SensorProfile!)
            .HasForeignKey(f => f.SensorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TelemetryLog>()
            .HasIndex(t => new { t.SensorId, t.Timestamp });

        modelBuilder.Entity<AnomalyLog>()
            .HasIndex(a => new { a.Severity, a.Timestamp });
    }
}
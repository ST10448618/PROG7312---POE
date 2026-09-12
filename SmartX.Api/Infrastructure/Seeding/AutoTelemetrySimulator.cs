using Microsoft.EntityFrameworkCore;
using SmartX.Api.Domain.ValueObjects;
using SmartX.Api.Infrastructure.Data;
using SmartX.Api.Services;

namespace SmartX.Api.Infrastructure.Seeding;

public class AutoTelemetrySimulator : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoTelemetrySimulator> _logger;
    private readonly Random _rnd = new();
    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(5);

    public AutoTelemetrySimulator(IServiceScopeFactory scopeFactory, ILogger<AutoTelemetrySimulator> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Give the API a moment to finish booting/migrating before the first tick.
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SimulateOneTickAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AutoTelemetrySimulator tick failed");
            }

            await Task.Delay(TickInterval, stoppingToken);
        }
    }

    private async Task SimulateOneTickAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ingestion = scope.ServiceProvider.GetRequiredService<TelemetryIngestionService>();

        var sensors = await db.Sensors.AsNoTracking().ToListAsync();
        if (sensors.Count == 0) return;

        // Each tick, pick one sensor to receive a reading — keeps the feed
        // readable on camera instead of flooding every sensor every 5s.
        var sensor = sensors[_rnd.Next(sensors.Count)];
        var roll = _rnd.NextDouble();

        switch (roll)
        {
            case < 0.75: // Normal
                await ingestion.IngestAsync(new TelemetryPacket<float>(
                    sensor.MacAddress, (float)Math.Round(_rnd.NextDouble() * 100, 2), "%"));
                break;

            case < 0.88: // Spike (Warning/Critical territory)
                await ingestion.IngestAsync(new TelemetryPacket<float>(
                    sensor.MacAddress, (float)(700 + _rnd.NextDouble() * 300), "%"));
                break;

            case < 0.95: // Drop
                await ingestion.IngestAsync(new TelemetryPacket<float>(
                    sensor.MacAddress, (float)(-(700 + _rnd.NextDouble() * 300)), "%"));
                break;

            default: // Disconnect
                await ingestion.MarkDisconnectedAsync(sensor.MacAddress);
                break;
        }
    }
}
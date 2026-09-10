using Microsoft.AspNetCore.SignalR;
using SmartX.Api.Domain.Collections;
using SmartX.Api.Domain.Entities;
using SmartX.Api.Domain.ValueObjects;
using SmartX.Api.Hubs;
using SmartX.Api.Infrastructure.Data;

namespace SmartX.Api.Services;

public class TelemetryIngestionService
{
    private readonly AppDbContext _db;
    private readonly AnomalyDetectionService _anomaly;
    private readonly TelemetryBatchStore _batches;
    private readonly IHubContext<TelemetryHub> _hub;

    public TelemetryIngestionService(
        AppDbContext db, AnomalyDetectionService anomaly, TelemetryBatchStore batches, IHubContext<TelemetryHub> hub)
    {
        _db = db;
        _anomaly = anomaly;
        _batches = batches;
        _hub = hub;
    }

    public async Task<AnomalyResult> IngestAsync<T>(TelemetryPacket<T> packet) where T : struct
    {
        _db.TelemetryLogs.Add(new TelemetryLog
        {
            SensorId = packet.SensorId,
            ValueType = typeof(T).Name,
            RawValue = packet.Value.ToString() ?? "",
            Unit = packet.Unit,
            Timestamp = packet.Timestamp
        });
        await _db.SaveChangesAsync();

        _batches.Append(packet.SensorId, packet.NumericValue, packet.Timestamp);

        var result = _anomaly.Score(packet.SensorId, packet.NumericValue);
        await _hub.Clients.All.SendAsync("AnomalyUpdate", result);
        return result;
    }
}
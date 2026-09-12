using SmartX.Api.Domain.Collections;
using SmartX.Api.Domain.ValueObjects;
using SmartX.Api.Dtos;
using SmartX.Api.Infrastructure.Data;
using SmartX.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace SmartX.Api.Endpoints;

public static class TelemetryEndpoints
{
    public static void MapTelemetryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/telemetry").WithTags("Telemetry");

        group.MapPost("/moisture", async (MoistureReadingRequest req, TelemetryIngestionService ingestion) =>
            Results.Ok(await ingestion.IngestAsync(new TelemetryPacket<float>(req.SensorId, req.Value, "%"))));

        group.MapPost("/power", async (PowerReadingRequest req, TelemetryIngestionService ingestion) =>
            Results.Ok(await ingestion.IngestAsync(new TelemetryPacket<int>(req.SensorId, req.Value, "W"))));

        group.MapPost("/valve", async (ValveReadingRequest req, TelemetryIngestionService ingestion) =>
            Results.Ok(await ingestion.IngestAsync(new TelemetryPacket<bool>(req.SensorId, req.Value, ""))));

        group.MapGet("/history/{sensorId}", (string sensorId, TelemetryBatchStore store) =>
            Results.Ok(store.GetOptimisedHistory(sensorId)));

        group.MapGet("/recent", async (AppDbContext db) =>
            Results.Ok(await db.TelemetryLogs.OrderByDescending(t => t.Timestamp).Take(20).ToListAsync()));

        group.MapPost("/{sensorId}/disconnect", async (string sensorId, TelemetryIngestionService ingestion) =>
            Results.Ok(await ingestion.MarkDisconnectedAsync(sensorId)));
    }
}
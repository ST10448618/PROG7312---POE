using SmartX.Api.Services;

namespace SmartX.Api.Endpoints;

public static class AnomalyLogEndpoints
{
    public static void MapAnomalyLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/anomalies").WithTags("AnomalyLogs");

        group.MapGet("/", async (AnomalyLogService svc, string? severity, string? search, int page = 1, int pageSize = 25) =>
            Results.Ok(await svc.SearchAsync(severity, search, page, pageSize)));

        group.MapPost("/{id}/acknowledge", async (int id, AnomalyLogService svc) =>
            Results.Ok(await svc.AcknowledgeAsync(id)));
    }
}
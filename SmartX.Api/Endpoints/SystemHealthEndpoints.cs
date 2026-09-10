using SmartX.Api.Services;

namespace SmartX.Api.Endpoints;

public static class SystemHealthEndpoints
{
    public static void MapSystemHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/system").WithTags("SystemHealth");
        group.MapGet("/health-summary", async (SystemHealthService svc) => Results.Ok(await svc.GetSummaryAsync()));
        group.MapGet("/incidents", async (SystemHealthService svc) => Results.Ok(await svc.GetRecentIncidentsAsync()));
    }
}
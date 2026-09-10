using SmartX.Api.Dtos;

namespace SmartX.Api.Endpoints;

public static class PowerEndpoints
{
    public static void MapPowerEndpoints(this WebApplication app)
    {
        app.MapPost("/api/power/aggregate", (PowerAggregateRequest req) =>
            Results.Ok((req.A + req.B).ToString())).WithTags("Power");
    }
}
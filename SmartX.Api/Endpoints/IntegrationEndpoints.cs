using SmartX.Api.Dtos;
using SmartX.Api.Services;
using static SmartX.Api.Infrastructure.Data.AppDbContext;

namespace SmartX.Api.Endpoints;

public static class IntegrationEndpoints
{
    public static void MapIntegrationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/integrations").WithTags("Integrations");

        group.MapGet("/", async (IntegrationService svc) => Results.Ok(await svc.GetAllAsync()));

        group.MapPost("/", async (RegisterIntegrationRequest req, IntegrationService svc) =>
            Results.Ok(await svc.RegisterAsync(req.Name, req.WebhookUrl)));

        group.MapPost("/{id}/test", async (int id, IntegrationService svc) =>
            Results.Ok(await svc.TestConnectionAsync(id)));

        group.MapDelete("/{id}", async (int id, IntegrationService svc) =>
        {
            await svc.DisconnectAsync(id);
            return Results.NoContent();
        });
    }
}
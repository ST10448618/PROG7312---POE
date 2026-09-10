using Microsoft.EntityFrameworkCore;
using SmartX.Api.Domain.Entities;
using SmartX.Api.Infrastructure.Data;

namespace SmartX.Api.Services;

public class IntegrationService
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpFactory;

    public IntegrationService(AppDbContext db, IHttpClientFactory httpFactory)
    {
        _db = db;
        _httpFactory = httpFactory;
    }

    public Task<List<IntegrationEndpoint>> GetAllAsync() =>
        _db.Integrations.AsNoTracking().ToListAsync();

    public async Task<IntegrationEndpoint> RegisterAsync(string name, string webhookUrl)
    {
        var integration = new IntegrationEndpoint { Name = name, WebhookUrl = webhookUrl };
        _db.Integrations.Add(integration);
        await _db.SaveChangesAsync();
        return integration;
    }

    /// <summary>Actually calls the webhook URL to verify it's reachable — real connectivity check, not a mock toggle.</summary>
    public async Task<IntegrationEndpoint> TestConnectionAsync(int id)
    {
        var integration = await _db.Integrations.FindAsync(id) ?? throw new KeyNotFoundException("Integration not found.");
        var client = _httpFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        try
        {
            var response = await client.PostAsJsonAsync(integration.WebhookUrl, new { type = "connection_test", source = "Smart-X" });
            integration.Status = response.IsSuccessStatusCode ? "Connected" : "Disconnected";
        }
        catch
        {
            integration.Status = "Disconnected";
        }

        integration.LastSyncAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return integration;
    }

    public async Task DisconnectAsync(int id)
    {
        var integration = await _db.Integrations.FindAsync(id) ?? throw new KeyNotFoundException("Integration not found.");
        integration.Status = "Disconnected";
        await _db.SaveChangesAsync();
    }
}
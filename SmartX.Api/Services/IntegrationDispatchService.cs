using Microsoft.EntityFrameworkCore;
using SmartX.Api.Infrastructure.Data;

namespace SmartX.Api.Services;

public class IntegrationDispatchService
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<IntegrationDispatchService> _logger;

    public IntegrationDispatchService(AppDbContext db, IHttpClientFactory httpFactory, ILogger<IntegrationDispatchService> logger)
    {
        _db = db;
        _httpFactory = httpFactory;
        _logger = logger;
    }

    public async Task NotifyAllAsync(string sensorId, double value, double score)
    {
        var connected = await _db.Integrations.Where(i => i.Status == "Connected").ToListAsync();
        if (connected.Count == 0) return;

        var client = _httpFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(3);
        var payload = new { sensorId, value, score, severity = "Critical", timestamp = DateTime.UtcNow };

        foreach (var integration in connected)
        {
            try
            {
                await client.PostAsJsonAsync(integration.WebhookUrl, payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed dispatching to integration {Name}", integration.Name);
            }
        }
    }
}
namespace SmartX.Api.Domain.Entities;

public class IntegrationEndpoint
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string WebhookUrl { get; set; } = "";
    public string Status { get; set; } = "Pending"; // Pending, Connected, Disconnected
    public DateTime? LastSyncAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
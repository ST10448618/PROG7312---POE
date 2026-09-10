namespace SmartX.Api.Domain.Entities;

public class AnomalyLog
{
    public int Id { get; set; }
    public string SensorId { get; set; } = "";
    public string Zone { get; set; } = "";
    public double Value { get; set; }
    public double Score { get; set; }
    public string Colour { get; set; } = "Grey";
    public string Severity { get; set; } = "Normal"; // Normal, Warning, Critical
    public bool Acknowledged { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
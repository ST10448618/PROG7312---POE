namespace SmartX.Api.Domain.Entities;

public class TelemetryLog
{
    public int Id { get; set; }
    public string SensorId { get; set; } = "";
    public string ValueType { get; set; } = "";
    public string RawValue { get; set; } = "";
    public string Unit { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
namespace SmartX.Api.Domain.ValueObjects;

public interface IAnomalyScorable
{
    string SensorId { get; }
    DateTime Timestamp { get; }
    double NumericValue { get; }
}

/// Generic wrapper so float/int/bool telemetry flow through one pipeline with
/// no boxing/unboxing. Constrained to struct so Convert.ToDouble is always safe.
public class TelemetryPacket<T> : IAnomalyScorable where T : struct
{
    public string SensorId { get; init; } = "";
    public T Value { get; init; }
    public string Unit { get; init; } = "";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public TelemetryPacket() { }

    public TelemetryPacket(string sensorId, T value, string unit = "")
    {
        SensorId = sensorId;
        Value = value;
        Unit = unit;
    }

    public double NumericValue => Convert.ToDouble(Value);

    public override string ToString() => $"[{Timestamp:HH:mm:ss}] {SensorId}: {Value}{Unit}";
}
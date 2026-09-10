namespace SmartX.Api.Domain.ValueObjects;

public class PowerReading : IComparable<PowerReading>, IEquatable<PowerReading>
{
    public string DeviceId { get; set; } = "";
    public double Watts { get; set; }

    public PowerReading() { }
    public PowerReading(string deviceId, double watts)
    {
        DeviceId = deviceId;
        Watts = watts;
    }

    public static PowerReading operator +(PowerReading a, PowerReading b)
        => new($"{a.DeviceId}+{b.DeviceId}", a.Watts + b.Watts);

    public static PowerReading operator -(PowerReading a, PowerReading b)
        => new($"{a.DeviceId}-{b.DeviceId}", Math.Abs(a.Watts - b.Watts));

    public static bool operator >(PowerReading a, PowerReading b) => a.Watts > b.Watts;
    public static bool operator <(PowerReading a, PowerReading b) => a.Watts < b.Watts;
    public static bool operator >=(PowerReading a, PowerReading b) => a.Watts >= b.Watts;
    public static bool operator <=(PowerReading a, PowerReading b) => a.Watts <= b.Watts;
}
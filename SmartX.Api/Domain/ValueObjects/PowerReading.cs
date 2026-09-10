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

     public static bool operator ==(PowerReading? a, PowerReading? b)
        => a is null ? b is null : a.Equals(b);
    public static bool operator !=(PowerReading? a, PowerReading? b) => !(a == b);

    public int CompareTo(PowerReading? other) => other is null ? 1 : Watts.CompareTo(other.Watts);
    public bool Equals(PowerReading? other) =>
        other is not null && DeviceId == other.DeviceId && Math.Abs(Watts - other.Watts) < 0.0001;
    public override bool Equals(object? obj) => Equals(obj as PowerReading);
    public override int GetHashCode() => HashCode.Combine(DeviceId, Watts);

    public override string ToString() => $"{DeviceId}: {Watts:0.0}W";
}
namespace SmartX.Api.Domain.Collections;

/// Singleton store of jagged-array telemetry batches per sensor. Raw ingestion
/// cycles are stored as double[] rows; GetOptimisedHistory flattens them into
/// a List&lt;double[]&gt; for reporting.
public class TelemetryBatchStore
{
    private readonly Dictionary<string, List<double[]>> _rawBatches = new();
    private readonly object _lock = new();

    public void Append(string sensorId, double value, DateTime timestamp)
    {
        lock (_lock)
        {
            if (!_rawBatches.TryGetValue(sensorId, out var batches))
            {
                batches = new List<double[]>();
                _rawBatches[sensorId] = batches;
            }
            batches.Add(new[] { value, timestamp.ToOADate() });
        }
    }

    public List<double[]> GetOptimisedHistory(string sensorId)
    {
        lock (_lock)
            return _rawBatches.TryGetValue(sensorId, out var batches) ? new List<double[]>(batches) : new();
    }

    public void SeedMockData(string sensorId, int batches = 50)
    {
        var rnd = new Random();
        for (var i = 0; i < batches; i++)
            Append(sensorId, Math.Round(rnd.NextDouble() * 100, 2), DateTime.UtcNow.AddMinutes(-i));
    }
}
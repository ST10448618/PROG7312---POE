using Microsoft.AspNetCore.SignalR;

namespace SmartX.Api.Hubs;

public class ConnectionTracker
{
    private int _count;
    public int Count => _count;
    public void Increment() => Interlocked.Increment(ref _count);
    public void Decrement() => Interlocked.Decrement(ref _count);
}

public class TelemetryHub : Hub
{
    private readonly ConnectionTracker _tracker;
    public TelemetryHub(ConnectionTracker tracker) => _tracker = tracker;

    public override Task OnConnectedAsync()
    {
        _tracker.Increment();
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _tracker.Decrement();
        return base.OnDisconnectedAsync(exception);
    }
}
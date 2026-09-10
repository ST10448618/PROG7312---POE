using Microsoft.AspNetCore.SignalR;

namespace SmartX.Api.Hubs;

public class TelemetryHub : Hub
{
    public async Task BroadcastPulse(string sensorId, string status)
        => await Clients.All.SendAsync("SensorPulse", sensorId, status);
}
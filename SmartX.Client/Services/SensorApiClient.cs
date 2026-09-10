using System.Net.Http.Json;
using SmartX.Client.Models;

namespace SmartX.Client.Services;

public class SensorApiClient
{
    private readonly HttpClient _http;
    public SensorApiClient(HttpClient http) => _http = http;

    public Task<List<SensorDto>?> GetSensorsAsync() =>
        _http.GetFromJsonAsync<List<SensorDto>>("api/sensors");

    public Task<SensorSummaryDto?> GetSummaryAsync() =>
        _http.GetFromJsonAsync<SensorSummaryDto>("api/sensors/summary");

    public Task<HttpResponseMessage> RegisterAsync(SensorDto sensor) =>
        _http.PostAsJsonAsync("api/sensors", sensor);

    public Task<HttpResponseMessage> UploadFileAsync(string mac, MultipartFormDataContent content) =>
        _http.PostAsync($"api/sensors/{mac}/upload", content);

    public async Task<(bool Success, AnomalyCellDto? Result, string? Error)> PushMoistureAsync(string sensorId, float value)
    {
        var response = await _http.PostAsJsonAsync("api/telemetry/moisture", new { sensorId, value });
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<AnomalyCellDto>(), null);

        return (false, null, await response.Content.ReadAsStringAsync());
    }

    public Task<HttpResponseMessage> ValidatePathAsync(string path) =>
        _http.PostAsJsonAsync("api/deployment/validate", new { path });
}
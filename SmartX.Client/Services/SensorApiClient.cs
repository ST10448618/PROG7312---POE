using System.Net.Http.Json;
using SmartX.Client.Models;

namespace SmartX.Client.Services;

public class SensorApiClient
{
    private readonly HttpClient _http;
    public SensorApiClient(HttpClient http) => _http = http;

    public Task<List<SensorDto>?> GetSensorsAsync() =>
        _http.GetFromJsonAsync<List<SensorDto>>("api/sensors");

    public Task<HttpResponseMessage> RegisterAsync(SensorDto sensor) =>
        _http.PostAsJsonAsync("api/sensors", sensor);

    public Task<HttpResponseMessage> UploadFileAsync(string mac, MultipartFormDataContent content) =>
        _http.PostAsync($"api/sensors/{mac}/upload", content);

    public Task<HttpResponseMessage> PushMoistureAsync(string sensorId, float value) =>
        _http.PostAsJsonAsync("api/telemetry/moisture", new { sensorId, value });

    public Task<HttpResponseMessage> ValidatePathAsync(string path) =>
        _http.PostAsJsonAsync("api/deployment/validate", new { path });
}
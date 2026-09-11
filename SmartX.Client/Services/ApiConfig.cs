namespace SmartX.Client.Services;

public class ApiConfig
{
    public string BaseUrl { get; }
    public ApiConfig(string baseUrl) => BaseUrl = baseUrl;
}
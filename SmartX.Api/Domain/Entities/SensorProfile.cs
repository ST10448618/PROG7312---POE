namespace SmartX.Api.Domain.Entities;

public class SensorProfile
{
    public int Id { get; set; }
    public string MacAddress { get; set; } = "";
    public string Location { get; set; } = "";
    public string Category { get; set; } = "";
    public string Status { get; set; } = "Online";
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public List<SensorFile> Files { get; set; } = new();
}
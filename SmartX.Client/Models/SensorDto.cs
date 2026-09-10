namespace SmartX.Client.Models;

public class SensorDto
{
    public int Id { get; set; }
    public string MacAddress { get; set; } = "";
    public string Location { get; set; } = "";
    public string Category { get; set; } = "";
    public string Status { get; set; } = "Online";
}

public class AnomalyCellDto
{
    public string SensorId { get; set; } = "";
    public double Value { get; set; }
    public double Score { get; set; }
    public string Colour { get; set; } = "Grey";
    public DateTime Timestamp { get; set; }
}
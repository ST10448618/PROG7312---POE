namespace SmartX.Api.Domain.Entities;

public class SensorFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public string StoredPath { get; set; } = "";
    public int SensorProfileId { get; set; }
    public SensorProfile? SensorProfile { get; set; }
}
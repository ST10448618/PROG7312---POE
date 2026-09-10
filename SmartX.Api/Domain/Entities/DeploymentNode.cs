namespace SmartX.Api.Domain.Entities;

public class DeploymentNode
{
    public string Name { get; set; } = "";
    public List<DeploymentNode> Children { get; set; } = new();
    public bool HasSensor { get; set; }
}
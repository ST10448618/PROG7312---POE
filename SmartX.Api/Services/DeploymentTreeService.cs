using System.Text.Json;
using SmartX.Api.Domain.Entities;

namespace SmartX.Api.Services;

public class DeploymentTreeService
{
    public DeploymentNode ParseFromJson(JsonElement element)
    {
        var node = new DeploymentNode
        {
            Name = element.GetProperty("name").GetString() ?? "",
            HasSensor = element.TryGetProperty("hasSensor", out var hs) && hs.GetBoolean()
        };

        if (element.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in children.EnumerateArray())
                node.Children.Add(ParseFromJson(child));
        }

        return node;
    }
    public bool ValidatePath(DeploymentNode node, Queue<string> path)
    {
        if (path.Count == 0)
            return node.HasSensor;

        var next = path.Dequeue();
        var child = node.Children.FirstOrDefault(c => c.Name.Equals(next, StringComparison.OrdinalIgnoreCase));
        return child is not null && ValidatePath(child, path);
    }

    public int CountNodes(DeploymentNode node) => 1 + node.Children.Sum(CountNodes);

    public DeploymentNode BuildDemoTree() => new()
    {
        Name = "Facility A",
        Children = new()
        {
            new DeploymentNode
            {
                Name = "Zone 1",
                Children = new()
                {
                    new DeploymentNode
                    {
                        Name = "Sub-Zone B",
                        Children = new() { new DeploymentNode { Name = "Rack 1", HasSensor = true } }
                    },
                    new DeploymentNode { Name = "Sub-Zone C", HasSensor = false }
                }
            }
        }
    };
}
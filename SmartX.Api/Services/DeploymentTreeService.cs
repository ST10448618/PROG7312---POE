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

}
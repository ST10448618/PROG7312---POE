using SmartX.Api.Dtos;
using SmartX.Api.Services;

namespace SmartX.Api.Endpoints;

public static class DeploymentEndpoints
{
    public static void MapDeploymentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/deployment").WithTags("Deployment");

        group.MapPost("/validate", (DeploymentPathRequest req, DeploymentTreeService trees) =>
        {
            var demoTree = trees.BuildDemoTree();
            var segments = new Queue<string>(req.Path.Split('/', StringSplitOptions.TrimEntries));
            var root = segments.Dequeue();
            var isValid = root.Equals(demoTree.Name, StringComparison.OrdinalIgnoreCase)
                && trees.ValidatePath(demoTree, segments);
            return Results.Ok(new { path = req.Path, isValid });
        });

        group.MapGet("/demo-tree/node-count", (DeploymentTreeService trees) =>
            Results.Ok(new { nodeCount = trees.CountNodes(trees.BuildDemoTree()) }));
    }
}
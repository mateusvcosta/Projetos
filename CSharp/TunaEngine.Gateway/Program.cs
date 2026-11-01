using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

var services = new[]
{
    new ServiceRoute(
        "members",
        "MembersServiceUrl",
        "https://tunaengine-membersservice.onrender.com/",
        ["/api/members/{**catch-all}", "/api/roles/{**catch-all}"]),
    new ServiceRoute(
        "events",
        "EventsServiceUrl",
        "https://tunaengine-eventsservice.onrender.com/",
        ["/api/events/{**catch-all}"]),
    new ServiceRoute(
        "documents",
        "DocumentsServiceUrl",
        "https://tunaengine-documentsservice.onrender.com/",
        ["/api/documents/{**catch-all}", "/api/doc-types/{**catch-all}"]),
    new ServiceRoute(
        "inventory",
        "InventoryServiceUrl",
        "https://tunaengine-inventoryservice.onrender.com/",
        ["/api/inventory/{**catch-all}"])
};

var routes = services
    .SelectMany(service => service.Paths.Select((path, index) => new RouteConfig
    {
        RouteId = $"{service.Name}-{index}",
        ClusterId = service.Name,
        Match = new RouteMatch { Path = path }
    }))
    .ToArray();

var clusters = services.Select(service => new ClusterConfig
{
    ClusterId = service.Name,
    Destinations = new Dictionary<string, DestinationConfig>
    {
        ["default"] = new DestinationConfig
        {
            Address = GetServiceUrl(service)
        }
    }
}).ToArray();

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(routes, clusters);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "TunaEngine.Gateway"
}));

app.MapReverseProxy();

app.Run();

static string GetServiceUrl(ServiceRoute service) =>
    Environment.GetEnvironmentVariable(service.EnvironmentVariable)
    ?? service.DefaultUrl;

sealed record ServiceRoute(
    string Name,
    string EnvironmentVariable,
    string DefaultUrl,
    string[] Paths);

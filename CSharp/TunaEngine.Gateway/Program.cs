using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

var services = new[]
{
    new ServiceRoute("members", "MembersServiceUrl", ["/api/members/{**catch-all}", "/api/roles/{**catch-all}"]),
    new ServiceRoute("events", "EventsServiceUrl", ["/api/events/{**catch-all}"]),
    new ServiceRoute("documents", "DocumentsServiceUrl", ["/api/documents/{**catch-all}", "/api/doc-types/{**catch-all}"]),
    new ServiceRoute("inventory", "InventoryServiceUrl", ["/api/inventory/{**catch-all}"])
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
            Address = GetRequiredServiceUrl(builder.Configuration, service)
        }
    }
}).ToArray();

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(routes, clusters);

var app = builder.Build();

app.MapGet("/health/live", () => Results.Ok(new { status = "ok", service = "TunaEngine.Gateway" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "ready", service = "TunaEngine.Gateway" }));

app.MapReverseProxy();

app.Run();

static string GetRequiredServiceUrl(IConfiguration configuration, ServiceRoute service)
{
    var address = configuration[service.ConfigurationKey];
    if (string.IsNullOrWhiteSpace(address))
        throw new InvalidOperationException($"{service.ConfigurationKey} configuration is missing.");

    return address;
}

sealed record ServiceRoute(
    string Name,
    string ConfigurationKey,
    string[] Paths);

using MongoDB.Driver;
using TunaEngine.EventsService.Interfaces;
using TunaEngine.EventsService.Models;
using TunaEngine.EventsService.Services;
using TunaEngine.EventsService.Settings;

var builder = WebApplication.CreateBuilder(args);
var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
    ?? throw new InvalidOperationException("MongoDB configuration is missing.");
if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
    throw new InvalidOperationException("MongoDB connection string is missing. Set MongoDb__ConnectionString.");
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.EventsDatabaseName).GetCollection<Events>(mongoSettings.EventsCollectionName));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.EventsDatabaseName).GetCollection<MongoDB.Bson.BsonDocument>("Sequences"));
builder.Services.AddSingleton<SequenceGenerator>();

builder.Services.AddScoped<EventsInterface, EventsService>();
builder.Services.AddControllers();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "TunaEngine.EventsService" }));
app.MapGet("/health/live", () => Results.Ok(new { status = "ok", service = "TunaEngine.EventsService" }));
app.MapGet("/health/ready", async () =>
{
    try
    {
        await app.Services.GetRequiredService<IMongoClient>()
            .GetDatabase(mongoSettings.EventsDatabaseName)
            .RunCommandAsync<MongoDB.Bson.BsonDocument>(
                MongoDB.Bson.BsonDocument.Parse("{ ping: 1 }"));
        return Results.Ok(new { status = "ready", service = "TunaEngine.EventsService" });
    }
    catch (Exception exception)
    {
        app.Logger.LogWarning(exception, "Readiness check failed.");
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
});
app.MapControllers();
app.Run();

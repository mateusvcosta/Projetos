using MongoDB.Driver;
using TunaEngine.DocumentsService.Interfaces;
using TunaEngine.DocumentsService.Models;
using TunaEngine.DocumentsService.Services;
using TunaEngine.DocumentsService.Settings;

var builder = WebApplication.CreateBuilder(args);
var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
    ?? throw new InvalidOperationException("MongoDB configuration is missing.");

if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    throw new InvalidOperationException(
        "MongoDB connection string is missing. Set MongoDb__ConnectionString.");
}

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>()
        .GetDatabase(mongoSettings.DocumentTypesDatabaseName)
        .GetCollection<DocumentType>(mongoSettings.DocumentTypesCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>()
        .GetDatabase(mongoSettings.DocumentsDatabaseName)
        .GetCollection<Documents>(mongoSettings.DocumentsCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>()
        .GetDatabase(mongoSettings.DocumentsDatabaseName)
        .GetCollection<MongoDB.Bson.BsonDocument>("Sequences"));
builder.Services.AddSingleton<SequenceGenerator>();
builder.Services.AddScoped<DocumentsInterface, DocumentsService>();
builder.Services.AddScoped<DocumentTypesInterface, DocumentTypesService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "TunaEngine.DocumentsService" }));
app.MapGet("/health/live", () => Results.Ok(new { status = "ok", service = "TunaEngine.DocumentsService" }));
app.MapGet("/health/ready", async () =>
{
    try
    {
        await app.Services.GetRequiredService<IMongoClient>()
            .GetDatabase(mongoSettings.DocumentsDatabaseName)
            .RunCommandAsync<MongoDB.Bson.BsonDocument>(
                MongoDB.Bson.BsonDocument.Parse("{ ping: 1 }"));
        return Results.Ok(new { status = "ready", service = "TunaEngine.DocumentsService" });
    }
    catch (Exception exception)
    {
        app.Logger.LogWarning(exception, "Readiness check failed.");
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
});
app.MapControllers();
app.Run();

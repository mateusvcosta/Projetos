using MongoDB.Driver;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;
using TunaEngine.Api.Services;
using TunaEngine.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration
    .GetSection(MongoDbSettings.SectionName)
    .Get<MongoDbSettings>() ?? new MongoDbSettings();

if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    throw new InvalidOperationException(
        "MongoDB connection string is missing. Set the MongoDb__ConnectionString environment variable.");
}

if (string.IsNullOrWhiteSpace(mongoSettings.DatabaseName)
    || string.IsNullOrWhiteSpace(mongoSettings.InventoryCollectionName))
{
    throw new InvalidOperationException(
        "MongoDb:DatabaseName and MongoDb:InventoryCollectionName must not be empty.");
}

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(
    _ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<InventoryItem>(mongoSettings.InventoryCollectionName));
builder.Services.AddScoped<InventoryInterface, InventoryService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

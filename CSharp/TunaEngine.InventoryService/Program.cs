using MongoDB.Driver;
using TunaEngine.InventoryService.Interfaces;
using TunaEngine.InventoryService.Models;
using TunaEngine.InventoryService.Services;
using TunaEngine.InventoryService.Settings;

var builder = WebApplication.CreateBuilder(args);
var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
    ?? throw new InvalidOperationException("MongoDB configuration is missing.");
if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
    throw new InvalidOperationException("MongoDB connection string is missing. Set MongoDb__ConnectionString.");
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.InventoryDatabaseName).GetCollection<InventoryItem>(mongoSettings.InventoryCollectionName));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.InventoryDatabaseName).GetCollection<MongoDB.Bson.BsonDocument>("Sequences"));
builder.Services.AddSingleton<SequenceGenerator>();

builder.Services.AddScoped<InventoryInterface, InventoryService>();
builder.Services.AddControllers();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "TunaEngine.InventoryService" }));
app.MapControllers();
app.Run();

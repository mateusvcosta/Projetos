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
app.MapControllers();
app.Run();

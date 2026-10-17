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
    || string.IsNullOrWhiteSpace(mongoSettings.InventoryCollectionName)
    || string.IsNullOrWhiteSpace(mongoSettings.RolesCollectionName)
    || string.IsNullOrWhiteSpace(mongoSettings.DocumentTypesCollectionName)
    || string.IsNullOrWhiteSpace(mongoSettings.DocumentsCollectionName)
    || string.IsNullOrWhiteSpace(mongoSettings.EventsCollectionName)
    || string.IsNullOrWhiteSpace(mongoSettings.MembersCollectionName))
{
    throw new InvalidOperationException(
        "MongoDB database and collection names must not be empty.");
}

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(
    _ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<InventoryItem>(mongoSettings.InventoryCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<Roles>(mongoSettings.RolesCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<DocumentType>(mongoSettings.DocumentTypesCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<Documents>(mongoSettings.DocumentsCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<Events>(mongoSettings.EventsCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<Members>(mongoSettings.MembersCollectionName));
builder.Services.AddScoped<InventoryInterface, InventoryService>();
builder.Services.AddScoped<RolesInterface, RolesService>();
builder.Services.AddScoped<DocumentTypesInterface, DocumentTypesService>();
builder.Services.AddScoped<DocumentsInterface, DocumentsService>();
builder.Services.AddScoped<EventsInterface, EventsService>();
builder.Services.AddScoped<MembersInterface, MembersService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

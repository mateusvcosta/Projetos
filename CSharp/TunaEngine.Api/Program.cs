using Microsoft.AspNetCore.Identity;
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

if (string.IsNullOrWhiteSpace(mongoSettings.InventoryDatabaseName)
    || string.IsNullOrWhiteSpace(mongoSettings.RolesDatabaseName)
    || string.IsNullOrWhiteSpace(mongoSettings.DocumentTypesDatabaseName)
    || string.IsNullOrWhiteSpace(mongoSettings.DocumentsDatabaseName)
    || string.IsNullOrWhiteSpace(mongoSettings.EventsDatabaseName)
    || string.IsNullOrWhiteSpace(mongoSettings.MembersDatabaseName)
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
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.InventoryDatabaseName)
        .GetCollection<InventoryItem>(mongoSettings.InventoryCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.RolesDatabaseName)
        .GetCollection<Roles>(mongoSettings.RolesCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DocumentTypesDatabaseName)
        .GetCollection<DocumentType>(mongoSettings.DocumentTypesCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DocumentsDatabaseName)
        .GetCollection<Documents>(mongoSettings.DocumentsCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.EventsDatabaseName)
        .GetCollection<Events>(mongoSettings.EventsCollectionName));
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.MembersDatabaseName)
        .GetCollection<Members>(mongoSettings.MembersCollectionName));
builder.Services.AddScoped<IPasswordHasher<Members>, PasswordHasher<Members>>();
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

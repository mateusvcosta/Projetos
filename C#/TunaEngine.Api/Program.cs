using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Api.Models;
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

var app = builder.Build();

app.MapGet("/health", async (
    IMongoClient mongoClient,
    MongoDbSettings settings,
    CancellationToken cancellationToken) =>
{
    await mongoClient
        .GetDatabase("admin")
        .RunCommandAsync<BsonDocument>(
            new BsonDocument("ping", 1),
            cancellationToken: cancellationToken);

    return Results.Ok(new { status = "ok", database = settings.DatabaseName });
});

app.MapGet("/api/inventory", async (
    IMongoCollection<InventoryItem> inventory,
    CancellationToken cancellationToken) =>
{
    var items = await inventory
        .Find(FilterDefinition<InventoryItem>.Empty)
        .SortByDescending(item => item.DateAdded)
        .ToListAsync(cancellationToken);

    return Results.Ok(items);
});

app.MapGet("/api/inventory/{id}", async (
    string id,
    IMongoCollection<InventoryItem> inventory,
    CancellationToken cancellationToken) =>
{
    if (!ObjectId.TryParse(id, out _))
    {
        return Results.BadRequest(new { error = "The inventory id must be a MongoDB ObjectId." });
    }

    var item = await inventory
        .Find(entry => entry.Id == id)
        .FirstOrDefaultAsync(cancellationToken);

    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/inventory", async (
    InventoryItemRequest request,
    IMongoCollection<InventoryItem> inventory,
    CancellationToken cancellationToken) =>
{
    var errors = request.GetValidationErrors();
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var id = ObjectId.GenerateNewId().ToString();
    var item = request.ToDocument(id, DateTime.UtcNow);
    await inventory.InsertOneAsync(item, cancellationToken: cancellationToken);

    return Results.Created($"/api/inventory/{id}", item);
});

app.MapPut("/api/inventory/{id}", async (
    string id,
    InventoryItemRequest request,
    IMongoCollection<InventoryItem> inventory,
    CancellationToken cancellationToken) =>
{
    if (!ObjectId.TryParse(id, out _))
    {
        return Results.BadRequest(new { error = "The inventory id must be a MongoDB ObjectId." });
    }

    var errors = request.GetValidationErrors();
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var update = Builders<InventoryItem>.Update
        .Set(item => item.Designation, request.Designation!.Trim())
        .Set(item => item.InstrumentType, request.InstrumentType?.Trim())
        .Set(item => item.Price, request.Price)
        .Set(item => item.Condition, request.Condition?.Trim())
        .Set(item => item.Function, request.Function?.Trim())
        .Set(item => item.Notes, request.Notes?.Trim());

    var result = await inventory.UpdateOneAsync(
        item => item.Id == id,
        update,
        cancellationToken: cancellationToken);

    return result.MatchedCount == 0 ? Results.NotFound() : Results.NoContent();
});

app.MapDelete("/api/inventory/{id}", async (
    string id,
    IMongoCollection<InventoryItem> inventory,
    CancellationToken cancellationToken) =>
{
    if (!ObjectId.TryParse(id, out _))
    {
        return Results.BadRequest(new { error = "The inventory id must be a MongoDB ObjectId." });
    }

    var result = await inventory.DeleteOneAsync(
        item => item.Id == id,
        cancellationToken);

    return result.DeletedCount == 0 ? Results.NotFound() : Results.NoContent();
});

app.Run();

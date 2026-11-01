using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.InventoryService.Interfaces;
using TunaEngine.InventoryService.Models;

namespace TunaEngine.InventoryService.Services;

public sealed class InventoryService(
    IMongoCollection<InventoryItem> inventory,
    SequenceGenerator sequences) : InventoryInterface
{
    public async Task<IReadOnlyList<InventoryItem>> GetAllInv(
        CancellationToken cancellationToken)
    {
        return await inventory
            .Find(FilterDefinition<InventoryItem>.Empty)
            .SortByDescending(item => item.DateAdded)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdInv(
        string id,
        CancellationToken cancellationToken)
    {
        return await inventory
            .Find(entry => entry.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InventoryItem> CreateItem(
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var inventoryCode = await sequences.NextAsync("inventory", "INV", cancellationToken);
        var item = request.ToDocument(id, inventoryCode, DateTime.UtcNow);
        await inventory.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> UpdateItem(
        string id,
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var update = Builders<InventoryItem>.Update
            .Set(item => item.Designation, request.Designation!.Trim())
            .Set(item => item.InstrumentType, request.InstrumentType?.Trim())
            .Set(item => item.Price, request.Price)
            .Set(item => item.Condition, request.Condition?.Trim())
            .Set(item => item.Function, request.Function?.Trim())
            .Set(item => item.Notes, request.Notes?.Trim())
            .Set(item => item.Brand, request.Brand?.Trim());

        var result = await inventory.UpdateOneAsync(
            item => item.Id == id,
            update,
            cancellationToken: cancellationToken);

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteItem(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await inventory.DeleteOneAsync(
            item => item.Id == id,
            cancellationToken);

        return result.DeletedCount > 0;
    }
}

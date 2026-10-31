using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Inventory.Api.Models;

namespace TunaEngine.Inventory.Api.Services;

public sealed class InventoryService(IMongoCollection<InventoryItem> items)
{
    public async Task<IReadOnlyList<InventoryItem>> GetAll(CancellationToken cancellationToken) =>
        await items.Find(Builders<InventoryItem>.Filter.Empty)
            .SortByDescending(item => item.DateAdded)
            .ToListAsync(cancellationToken);

    public async Task<InventoryItem?> GetById(string id, CancellationToken cancellationToken) =>
        await items.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<InventoryItem> Create(InventoryItemRequest request, CancellationToken cancellationToken)
    {
        var codes = await items.Find(Builders<InventoryItem>.Filter.Empty)
            .Project(item => item.InventoryCode)
            .ToListAsync(cancellationToken);
        var nextNumber = codes.Select(code => code.StartsWith("INV-", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(code.AsSpan(4), out var number) ? number : 0)
            .DefaultIfEmpty()
            .Max() + 1;
        var item = request.ToDocument(ObjectId.GenerateNewId().ToString(), $"INV-{nextNumber:0000}");
        await items.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> Update(string id, InventoryItemRequest request, CancellationToken cancellationToken)
    {
        var existing = await GetById(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Designation = request.Designation!.Trim();
        existing.InstrumentType = request.InstrumentType?.Trim();
        existing.Price = request.Price;
        existing.Condition = request.Condition?.Trim();
        existing.Brand = request.Brand!.Trim();
        existing.Function = request.Function?.Trim();
        existing.Notes = request.Notes?.Trim();

        await items.ReplaceOneAsync(item => item.Id == id, existing, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await items.DeleteOneAsync(item => item.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }
}

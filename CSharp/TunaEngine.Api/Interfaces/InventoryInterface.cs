using TunaEngine.Api.Models;

namespace TunaEngine.Api.Interfaces;

public interface InventoryInterface
{
    Task<IReadOnlyList<InventoryItem>> GetAllInv(CancellationToken cancellationToken);

    Task<InventoryItem?> GetByIdInv(string id, CancellationToken cancellationToken);

    Task<InventoryItem> CreateItem(
        InventoryItemRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateItem(
        string id,
        InventoryItemRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteItem(string id, CancellationToken cancellationToken);
}

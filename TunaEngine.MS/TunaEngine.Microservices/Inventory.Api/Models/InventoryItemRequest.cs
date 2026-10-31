using System.ComponentModel.DataAnnotations;

namespace TunaEngine.Inventory.Api.Models;

public sealed class InventoryItemRequest
{
    [Required, StringLength(120)]
    public string? Designation { get; init; }

    [StringLength(80)]
    public string? InstrumentType { get; init; }

    [Range(0, 1000000)]
    public decimal? Price { get; init; }

    [StringLength(80)]
    public string? Condition { get; init; }

    [StringLength(120)]
    public string? Function { get; init; }

    [Required, StringLength(80)]
    public string? Brand { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }

    public InventoryItem ToDocument(string id, string code) => new()
    {
        Id = id,
        InventoryCode = code,
        DateAdded = DateTime.UtcNow,
        Designation = Designation!.Trim(),
        InstrumentType = InstrumentType?.Trim(),
        Price = Price,
        Condition = Condition?.Trim(),
        Function = Function?.Trim(),
        Brand = Brand!.Trim(),
        Notes = Notes?.Trim()
    };
}

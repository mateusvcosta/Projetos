namespace TunaEngine.Api.Models;

public sealed class InventoryItemRequest
{
    public string? Designation { get; init; }
    public string? InstrumentType { get; init; }
    public decimal? Price { get; init; }
    public string? Condition { get; init; }
    public string? Function { get; init; }
    public string? Notes { get; init; }

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Designation))
        {
            errors[nameof(Designation)] = ["Designation is required."];
        }

        if (Price is < 0)
        {
            errors[nameof(Price)] = ["Price cannot be negative."];
        }

        return errors;
    }

    public InventoryItem ToDocument(string id, DateTime dateAdded) => new()
    {
        Id = id,
        InventoryCode = $"INV-{id}",
        DateAdded = dateAdded,
        Designation = Designation!.Trim(),
        InstrumentType = InstrumentType?.Trim(),
        Price = Price,
        Condition = Condition?.Trim(),
        Function = Function?.Trim(),
        Notes = Notes?.Trim()
    };
}

namespace TunaEngine.Api.Models;

public sealed class RolesRequest
{
    public string? Id_Cargo { get; init; }
    public string? Cargo_Type { get; init; }

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Cargo_Type))
        {
            errors[nameof(Cargo_Type)] = ["Cargo type is required."];
        }

        return errors;
    }

    public Roles ToDocument(string id)
    {
        var cargoId = string.IsNullOrWhiteSpace(Id_Cargo)
            ? $"CAR-{id}"
            : Id_Cargo.Trim();

        return new Roles
        {
            Id = id,
            Id_Cargo = cargoId,
            Cargo_Type = Cargo_Type!.Trim()
        };
    }
}

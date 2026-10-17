namespace TunaEngine.Api.Models;

public sealed class DocumentTypeRequest
{
    public string? Id_Doc_Type { get; init; }
    public string? Doc_type { get; init; }

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Id_Doc_Type))
        {
            errors[nameof(Id_Doc_Type)] = ["Document type id is required."];
        }

        if (string.IsNullOrWhiteSpace(Doc_type))
        {
            errors[nameof(Doc_type)] = ["Document type is required."];
        }

        return errors;
    }

    public DocumentType ToDocument(string id) => new()
    {
        Id = id,
        Id_Doc_Type = Id_Doc_Type!.Trim(),
        Doc_type = Doc_type!.Trim()
    };
}

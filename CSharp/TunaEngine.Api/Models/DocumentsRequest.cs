namespace TunaEngine.Api.Models;

public sealed class DocumentsRequest
{
    public string? Id_Documento { get; init; }
    public string? Tipo_Documento { get; init; }
    public string? Id_Doc_Type { get; init; }
    public string? File_Path { get; init; }
    public DateTimeOffset? Date_Created { get; init; }
    public string? Added_By { get; init; }
    public string? Obs { get; init; }

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Id_Documento))
        {
            errors[nameof(Id_Documento)] = ["Document id is required."];
        }

        if (string.IsNullOrWhiteSpace(Tipo_Documento))
        {
            errors[nameof(Tipo_Documento)] = ["Document type is required."];
        }

        if (string.IsNullOrWhiteSpace(Id_Doc_Type))
        {
            errors[nameof(Id_Doc_Type)] = ["Document type id is required."];
        }

        if (Date_Created is null)
        {
            errors[nameof(Date_Created)] = ["Creation date is required."];
        }

        return errors;
    }

    public Documents ToDocument(string id) => new()
    {
        Id = id,
        Id_Documento = Id_Documento!.Trim(),
        Tipo_Documento = Tipo_Documento!.Trim(),
        Id_Doc_Type = Id_Doc_Type!.Trim(),
        File_Path = File_Path?.Trim() ?? "",
        Date_Created = Date_Created!.Value.UtcDateTime,
        Added_By = Added_By?.Trim() ?? "",
        Obs = Obs?.Trim() ?? ""
    };
}

using System.Text.Json.Serialization;

namespace TunaEngine.Services;

public sealed class ApiInventoryItem
{
    public string Id { get; set; } = "";
    public string Designation { get; set; } = "";
    public string? InstrumentType { get; set; }
    public decimal? Price { get; set; }
}

public sealed class ApiMember
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? RoleId { get; set; }
}

public sealed class ApiRole
{
    public string Id { get; set; } = "";

    [JsonPropertyName("id_Cargo")]
    public string RoleCode { get; set; } = "";

    [JsonPropertyName("cargo_Type")]
    public string Name { get; set; } = "";
}

public sealed class ApiEvent
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public string Location { get; set; } = "";

    [JsonPropertyName("date_Hour")]
    public DateTime Date { get; set; }
}

public sealed class ApiDocument
{
    public string Id { get; set; } = "";

    [JsonPropertyName("tipo_Documento")]
    public string Type { get; set; } = "";

    [JsonPropertyName("file_Path")]
    public string FilePath { get; set; } = "";

    [JsonPropertyName("date_Created")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("added_By")]
    public string AddedBy { get; set; } = "";

    [JsonPropertyName("obs")]
    public string Notes { get; set; } = "";
}

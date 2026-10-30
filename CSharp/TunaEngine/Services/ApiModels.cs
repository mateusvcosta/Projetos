using System.Text.Json.Serialization;

namespace TunaEngine.Services;

public sealed class ApiInventoryItem
{
    public string Id { get; set; } = "";
    public string Designation { get; set; } = "";
    public string? InstrumentType { get; set; }
    public string? Brand { get; set; }
    public float? Price { get; set; }
    public string? Condition { get; set; }
    public string? Function { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

public sealed class ApiInventoryItemRequest
{
    public string Designation { get; init; } = "";
    public string? InstrumentType { get; init; }
    public string? Brand { get; init; }
    public float? Price { get; init; }
    public string? Condition { get; init; }
    public string? Function { get; init; }
    public string? Notes { get; init; }
}

public sealed class ApiMember
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("id_Member")]
    public string? MemberCode { get; set; }

    public string Name { get; set; } = "";

    [JsonPropertyName("tuna_Name")]
    public string? TunaName { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }

    [JsonPropertyName("num_Student")]
    public string? StudentNumber { get; set; }

    [JsonPropertyName("id_Role")]
    public string? RoleId { get; set; }

    public string? VoiceType { get; set; }

    [JsonPropertyName("voice_Type")]
    public string? LegacyVoiceType { get; set; }

    public string? Instrument { get; set; }
    public string? Password { get; set; }
}

public sealed class ApiLoginRequest
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}

public sealed class ApiLoginResponse
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public string? RoleId { get; init; }
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

public sealed class ApiDocumentRequest
{
    [JsonPropertyName("id_Documento")]
    public string DocumentCode { get; init; } = "";

    [JsonPropertyName("tipo_Documento")]
    public string Type { get; init; } = "";

    [JsonPropertyName("id_Doc_Type")]
    public string DocumentTypeCode { get; init; } = "";

    [JsonPropertyName("file_Path")]
    public string? FilePath { get; init; }

    [JsonPropertyName("date_Created")]
    public DateTime DateCreated { get; init; }

    [JsonPropertyName("added_By")]
    public string? AddedBy { get; init; }

    [JsonPropertyName("obs")]
    public string? Notes { get; init; }
}

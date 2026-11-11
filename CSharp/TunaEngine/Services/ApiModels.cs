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

    [JsonPropertyName("Num_Studentt")]
    public string? LegacyStudentNumber { get; set; }

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

    [JsonPropertyName("id_Event")]
    public string? EventCode { get; set; }

    public List<ApiEventParticipant> Participants { get; set; } = [];
    public List<ApiEventInstrument> Instruments_Needed { get; set; } = [];
    public List<ApiEventRide> Boleias { get; set; } = [];
}

public sealed class ApiEventRequest
{
    [JsonPropertyName("id_Event")]
    public string EventCode { get; init; } = "";

    [JsonPropertyName("date_Hour")]
    public DateTimeOffset Date { get; init; }

    public string Name { get; init; } = "";
    public string Location { get; init; } = "";

    public List<ApiEventParticipant> Participants { get; init; } = [];
    public List<ApiEventInstrument> Instruments_Needed { get; init; } = [];
    public List<ApiEventRide> Boleias { get; init; } = [];
}

public sealed class ApiEventParticipant
{
    [JsonPropertyName("id_Member")]
    public string MemberId { get; init; } = "";

    [JsonPropertyName("attendance_Status")]
    public string AttendanceStatus { get; init; } = "";
}

public sealed class ApiEventInstrument
{
    [JsonPropertyName("id_Inventory")]
    public string InventoryId { get; init; } = "";

    public string Designation { get; init; } = "";
    public int Quantity { get; init; }
}

public sealed class ApiEventRide
{
    [JsonPropertyName("id_Driver")]
    public string DriverId { get; init; } = "";

    [JsonPropertyName("pickup_Location")]
    public string PickupLocation { get; init; } = "";

    [JsonPropertyName("departure_Hour")]
    public string DepartureHour { get; init; } = "";

    [JsonPropertyName("available_Seats")]
    public int AvailableSeats { get; init; }

    [JsonPropertyName("passenger_Ids")]
    public List<string> PassengerIds { get; init; } = [];
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

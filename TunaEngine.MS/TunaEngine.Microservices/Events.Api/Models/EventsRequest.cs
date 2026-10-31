using System.ComponentModel.DataAnnotations;

namespace TunaEngine.Events.Api.Models;

public sealed class EventsRequest
{
    [Required]
    public string? Id_Event { get; init; }

    [Required]
    public DateTimeOffset? Date_Hour { get; init; }

    [Required]
    public string? Name { get; init; }

    [Required]
    public string? Location { get; init; }

    [Range(1, int.MaxValue)]
    public int? GlobalIdLocal { get; init; }

    public List<EventParticipant> Participants { get; init; } = [];
    public List<EventInstrument> Instruments_Needed { get; init; } = [];
    public List<EventRide> Boleias { get; init; } = [];

    public Events ToDocument(string id) => new()
    {
        Id = id,
        Id_Event = Id_Event!.Trim(),
        Date_Hour = Date_Hour!.Value.UtcDateTime,
        Name = Name!.Trim(),
        Location = Location!.Trim(),
        GlobalIdLocal = GlobalIdLocal,
        Participants = Participants,
        Instruments_Needed = Instruments_Needed,
        Boleias = Boleias
    };
}

public sealed class AttendanceRequest
{
    [Required]
    public string? Status { get; init; }
}

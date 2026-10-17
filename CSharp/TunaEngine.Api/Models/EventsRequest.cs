namespace TunaEngine.Api.Models;

public sealed class EventsRequest
{
    public string? Id_Event { get; init; }
    public DateTimeOffset? Date_Hour { get; init; }
    public string? Name { get; init; }
    public string? Location { get; init; }
    public List<EventParticipant> Participants { get; init; } = [];
    public List<EventInstrument> Instruments_Needed { get; init; } = [];
    public List<EventRide> Boleias { get; init; } = [];

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Id_Event))
        {
            errors[nameof(Id_Event)] = ["Event id is required."];
        }

        if (Date_Hour is null)
        {
            errors[nameof(Date_Hour)] = ["Event date and time are required."];
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            errors[nameof(Name)] = ["Event name is required."];
        }

        if (string.IsNullOrWhiteSpace(Location))
        {
            errors[nameof(Location)] = ["Event location is required."];
        }

        if (Instruments_Needed.Any(instrument => instrument.Quantity < 0))
        {
            errors[nameof(Instruments_Needed)] = ["Instrument quantities cannot be negative."];
        }

        if (Boleias.Any(ride => ride.Available_Seats < 0))
        {
            errors[nameof(Boleias)] = ["Available seats cannot be negative."];
        }

        return errors;
    }

    public Events ToDocument(string id) => new()
    {
        Id = id,
        Id_Event = Id_Event!.Trim(),
        Date_Hour = Date_Hour!.Value.UtcDateTime,
        Name = Name!.Trim(),
        Location = Location!.Trim(),
        Participants = Participants,
        Instruments_Needed = Instruments_Needed,
        Boleias = Boleias
    };
}

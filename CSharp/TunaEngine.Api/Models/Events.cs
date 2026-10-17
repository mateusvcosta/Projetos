using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Api.Models;

public sealed class Events
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Id_Event")]
    public string Id_Event { get; set; } = "";

    [BsonElement("Date_Hour")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date_Hour { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = "";

    [BsonElement("Location")]
    public string Location { get; set; } = "";

    [BsonElement("Participants")]
    public List<EventParticipant> Participants { get; set; } = [];

    [BsonElement("Instruments_Needed")]
    public List<EventInstrument> Instruments_Needed { get; set; } = [];

    [BsonElement("Boleias")]
    public List<EventRide> Boleias { get; set; } = [];
}

public sealed class EventParticipant
{
    [BsonElement("Id_Member")]
    public string Id_Member { get; set; } = "";

    [BsonElement("Attendance_Status")]
    public string Attendance_Status { get; set; } = "";
}

public sealed class EventInstrument
{
    [BsonElement("Id_Inventory")]
    public string Id_Inventory { get; set; } = "";

    [BsonElement("Designation")]
    public string Designation { get; set; } = "";

    [BsonElement("Quantity")]
    public int Quantity { get; set; }
}

public sealed class EventRide
{
    [BsonElement("Id_Driver")]
    public string Id_Driver { get; set; } = "";

    [BsonElement("Pickup_Location")]
    public string Pickup_Location { get; set; } = "";

    [BsonElement("Departure_Hour")]
    public string Departure_Hour { get; set; } = "";

    [BsonElement("Available_Seats")]
    public int Available_Seats { get; set; }

    [BsonElement("Passenger_Ids")]
    public List<string> Passenger_Ids { get; set; } = [];
}

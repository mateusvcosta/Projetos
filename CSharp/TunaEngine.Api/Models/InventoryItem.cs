using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Api.Models;

public sealed class InventoryItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Id_inventory")]
    public string InventoryCode { get; set; } = "";

    [BsonElement("Date_added")]
    public DateTime DateAdded { get; set; }

    [BsonElement("Designation")]
    public string Designation { get; set; } = "";

    [BsonElement("Instrument_Type")]
    public string? InstrumentType { get; set; }

    [BsonElement("Price")]
    public decimal? Price { get; set; }

    [BsonElement("Condition")]
    public string? Condition { get; set; }

    [BsonElement("Function")]
    public string? Function { get; set; }

    [BsonElement("Obs")]
    public string? Notes { get; set; }
}

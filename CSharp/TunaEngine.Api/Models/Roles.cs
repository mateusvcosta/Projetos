using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Api.Models;

public sealed class Roles
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Id_Cargo")]
    public string Id_Cargo { get; set; } = "";

    [BsonElement("Cargo_Type")]
    public string Cargo_Type { get; set; } = "";
}

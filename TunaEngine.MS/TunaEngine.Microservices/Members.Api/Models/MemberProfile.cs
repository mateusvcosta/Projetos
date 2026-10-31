using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Members.Api.Models;

public sealed class MemberProfile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Name")]
    public string Name { get; set; } = "";

    [BsonElement("Email")]
    public string Email { get; set; } = "";

    [BsonElement("Tuna_Name")]
    public string TunaName { get; set; } = "";

    [BsonElement("Phone")]
    public string? Phone { get; set; }

    [BsonElement("Num_Student")]
    public string? StudentNumber { get; set; }

    [BsonElement("Role")]
    public string Role { get; set; } = "Member";

    [BsonElement("Voice_Type")]
    public string? VoiceType { get; set; }

    [BsonElement("Instrument")]
    public string? Instrument { get; set; }
}

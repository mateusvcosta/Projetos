using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TunaEngine.Api.Models;

public sealed class Members
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Id_Member")]
    public string MemberId { get; set; } = "";

    [BsonElement("Date_added")]
    public DateTime DateAdded { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = "";

    [BsonElement("Tuna_Name")]
    public string TunaName { get; set; } = "";

    [BsonElement("Email")]
    public string Email { get; set; } = "";

    [BsonElement("PasswordHash")]
    [JsonIgnore]
    public string? PasswordHash { get; set; }

    [BsonElement("Phone")]
    public string? Phone { get; set; }

    [BsonElement("Num_Studentt")]
    public string? StudentNumber { get; set; }

    [BsonElement("Id_Role")]
    public string? RoleId { get; set; }

    [BsonElement("Voice_Type")]
    public string? VoiceType { get; set; }

    [BsonElement("Instrument")]
    public string? Instrument { get; set; }

    [BsonElement("Password")]
    public string? Password { get; set; }
}

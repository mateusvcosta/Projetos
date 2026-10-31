using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Auth.Api.Models;

public sealed class AuthUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Name")]
    public string Name { get; set; } = "";

    [BsonElement("Email")]
    public string Email { get; set; } = "";

    [BsonElement("PasswordHash")]
    public string PasswordHash { get; set; } = "";
}

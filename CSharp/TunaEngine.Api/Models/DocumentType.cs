using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Api.Models;

public sealed class DocumentType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Id_Doc_Type")]
    public string Id_Doc_Type { get; set; } = "";

    [BsonElement("Doc_type")]
    public string Doc_type { get; set; } = "";
}

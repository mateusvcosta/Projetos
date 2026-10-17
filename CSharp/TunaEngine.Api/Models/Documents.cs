using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TunaEngine.Api.Models;

public sealed class Documents
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("Id_Documento")]
    public string Id_Documento { get; set; } = "";

    [BsonElement("Tipo_Documento")]
    public string Tipo_Documento { get; set; } = "";

    [BsonElement("Id_Doc_Type")]
    public string Id_Doc_Type { get; set; } = "";

    [BsonElement("File_Path")]
    public string File_Path { get; set; } = "";

    [BsonElement("Date_Created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date_Created { get; set; }

    [BsonElement("Added_By")]
    public string Added_By { get; set; } = "";

    [BsonElement("Obs")]
    public string Obs { get; set; } = "";
}

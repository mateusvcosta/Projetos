using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Services;

public sealed class DocumentsService(
    IMongoCollection<Documents> documents) : DocumentsInterface
{
    public async Task<IReadOnlyList<Documents>> GetAll(CancellationToken cancellationToken) =>
        await documents.Find(FilterDefinition<Documents>.Empty)
            .ToListAsync(cancellationToken);

    public async Task<Documents?> GetById(string id, CancellationToken cancellationToken) =>
        await documents.Find(item => item.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Documents> Create(
        DocumentsRequest request,
        CancellationToken cancellationToken)
    {
        var item = request.ToDocument(ObjectId.GenerateNewId().ToString());
        await documents.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> Update(
        string id,
        DocumentsRequest request,
        CancellationToken cancellationToken)
    {
        var update = Builders<Documents>.Update
            .Set(item => item.Id_Documento, request.Id_Documento!.Trim())
            .Set(item => item.Tipo_Documento, request.Tipo_Documento!.Trim())
            .Set(item => item.Id_Doc_Type, request.Id_Doc_Type!.Trim())
            .Set(item => item.File_Path, request.File_Path?.Trim() ?? "")
            .Set(item => item.Date_Created, request.Date_Created!.Value.UtcDateTime)
            .Set(item => item.Added_By, request.Added_By?.Trim() ?? "")
            .Set(item => item.Obs, request.Obs?.Trim() ?? "");
        var result = await documents.UpdateOneAsync(
            item => item.Id == id, update, cancellationToken: cancellationToken);
        return result.MatchedCount > 0;
    }

    public async Task<bool> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await documents.DeleteOneAsync(
            item => item.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }
}

using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Services;

public sealed class DocumentTypesService(
    IMongoCollection<DocumentType> documentTypes) : DocumentTypesInterface
{
    public async Task<IReadOnlyList<DocumentType>> GetAll(CancellationToken cancellationToken) =>
        await documentTypes.Find(FilterDefinition<DocumentType>.Empty)
            .ToListAsync(cancellationToken);

    public async Task<DocumentType?> GetById(string id, CancellationToken cancellationToken) =>
        await documentTypes.Find(item => item.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<DocumentType> Create(
        DocumentTypeRequest request,
        CancellationToken cancellationToken)
    {
        var item = request.ToDocument(ObjectId.GenerateNewId().ToString());
        await documentTypes.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> Update(
        string id,
        DocumentTypeRequest request,
        CancellationToken cancellationToken)
    {
        var update = Builders<DocumentType>.Update
            .Set(item => item.Id_Doc_Type, request.Id_Doc_Type!.Trim())
            .Set(item => item.Doc_type, request.Doc_type!.Trim());
        var result = await documentTypes.UpdateOneAsync(
            item => item.Id == id, update, cancellationToken: cancellationToken);
        return result.MatchedCount > 0;
    }

    public async Task<bool> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await documentTypes.DeleteOneAsync(
            item => item.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }
}

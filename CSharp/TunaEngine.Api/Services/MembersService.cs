using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Services;

public sealed class MembersService(
    IMongoCollection<Members> members) : MembersInterface
{
    public async Task<IReadOnlyList<Members>> GetAllMembers(
        CancellationToken cancellationToken)
    {
        return await members
            .Find(FilterDefinition<Members>.Empty)
            .SortByDescending(member => member.DateAdded)
            .ToListAsync(cancellationToken);
    }

    public async Task<Members?> GetByIdMember(
        string id,
        CancellationToken cancellationToken)
    {
        return await members
            .Find(member => member.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Members> CreateMember(
        MembersRequest request,
        CancellationToken cancellationToken)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var item = request.ToDocument(id, DateTime.UtcNow);
        await members.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> UpdateMember(
        string id,
        MembersRequest request,
        CancellationToken cancellationToken)
    {
        var update = Builders<Members>.Update
            .Set(member => member.Name, request.Name!.Trim())
            .Set(member => member.Email, request.Email!.Trim())
            .Set(member => member.Phone, request.Phone?.Trim())
            .Set(member => member.Notes, request.Notes?.Trim());

        var result = await members.UpdateOneAsync(
            member => member.Id == id,
            update,
            cancellationToken: cancellationToken);

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteMember(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await members.DeleteOneAsync(
            member => member.Id == id,
            cancellationToken);

        return result.DeletedCount > 0;
    }
}

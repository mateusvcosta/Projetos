using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Services;

public sealed class RolesService(
    IMongoCollection<Roles> roles) : RolesInterface
{
    public async Task<IReadOnlyList<Roles>> GetAllRoles(
        CancellationToken cancellationToken)
    {
        return await roles
            .Find(FilterDefinition<Roles>.Empty)
            .ToListAsync(cancellationToken);
    }

    public async Task<Roles?> GetByIdRole(
        string id,
        CancellationToken cancellationToken)
    {
        return await roles
            .Find(role => role.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Roles> CreateRole(
        RolesRequest request,
        CancellationToken cancellationToken)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var item = request.ToDocument(id);
        await roles.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> UpdateRole(
        string id,
        RolesRequest request,
        CancellationToken cancellationToken)
    {
        var updates = new List<UpdateDefinition<Roles>>();

        if (!string.IsNullOrWhiteSpace(request.Id_Cargo))
        {
            updates.Add(Builders<Roles>.Update.Set(role => role.Id_Cargo, request.Id_Cargo.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(request.Cargo_Type))
        {
            updates.Add(Builders<Roles>.Update.Set(role => role.Cargo_Type, request.Cargo_Type.Trim()));
        }

        if (updates.Count == 0)
        {
            return false;
        }

        var result = await roles.UpdateOneAsync(
            role => role.Id == id,
            Builders<Roles>.Update.Combine(updates),
            cancellationToken: cancellationToken);

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteRole(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await roles.DeleteOneAsync(
            role => role.Id == id,
            cancellationToken);

        return result.DeletedCount > 0;
    }
}

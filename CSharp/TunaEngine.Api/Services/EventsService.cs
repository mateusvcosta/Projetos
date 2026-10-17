using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Services;

public sealed class EventsService(
    IMongoCollection<Events> events) : EventsInterface
{
    public async Task<IReadOnlyList<Events>> GetAll(CancellationToken cancellationToken) =>
        await events.Find(FilterDefinition<Events>.Empty)
            .ToListAsync(cancellationToken);

    public async Task<Events?> GetById(string id, CancellationToken cancellationToken) =>
        await events.Find(item => item.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Events> Create(
        EventsRequest request,
        CancellationToken cancellationToken)
    {
        var item = request.ToDocument(ObjectId.GenerateNewId().ToString());
        await events.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> Update(
        string id,
        EventsRequest request,
        CancellationToken cancellationToken)
    {
        var update = Builders<Events>.Update
            .Set(item => item.Id_Event, request.Id_Event!.Trim())
            .Set(item => item.Date_Hour, request.Date_Hour!.Value.UtcDateTime)
            .Set(item => item.Name, request.Name!.Trim())
            .Set(item => item.Location, request.Location!.Trim())
            .Set(item => item.Participants, request.Participants)
            .Set(item => item.Instruments_Needed, request.Instruments_Needed)
            .Set(item => item.Boleias, request.Boleias);
        var result = await events.UpdateOneAsync(
            item => item.Id == id, update, cancellationToken: cancellationToken);
        return result.MatchedCount > 0;
    }

    public async Task<bool> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await events.DeleteOneAsync(
            item => item.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }
}

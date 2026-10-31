using MongoDB.Bson;
using MongoDB.Driver;
using TunaEngine.Events.Api.Models;
using EventDocument = TunaEngine.Events.Api.Models.Events;

namespace TunaEngine.Events.Api.Services;

public sealed class EventsService(IMongoCollection<EventDocument> events)
{
    public async Task<IReadOnlyList<EventDocument>> GetAll(CancellationToken cancellationToken) =>
        await events.Find(Builders<EventDocument>.Filter.Empty).SortBy(item => item.Date_Hour)
            .ToListAsync(cancellationToken);

    public async Task<EventDocument?> GetById(string id, CancellationToken cancellationToken) =>
        await events.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<EventDocument> Create(EventsRequest request, CancellationToken cancellationToken)
    {
        var item = request.ToDocument(ObjectId.GenerateNewId().ToString());
        await events.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> Update(string id, EventsRequest request, CancellationToken cancellationToken)
    {
        var result = await events.ReplaceOneAsync(
            item => item.Id == id,
            request.ToDocument(id),
            cancellationToken: cancellationToken);
        return result.MatchedCount > 0;
    }

    public async Task<bool> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await events.DeleteOneAsync(item => item.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<EventDocument?> SetAttendance(
        string id,
        string memberId,
        string status,
        CancellationToken cancellationToken)
    {
        var item = await GetById(id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var participant = item.Participants.FirstOrDefault(entry => entry.Id_Member == memberId);
        if (participant is null)
        {
            item.Participants.Add(new EventParticipant
            {
                Id_Member = memberId,
                Attendance_Status = status
            });
        }
        else
        {
            participant.Attendance_Status = status;
        }

        await events.ReplaceOneAsync(entry => entry.Id == id, item, cancellationToken: cancellationToken);
        return item;
    }
}

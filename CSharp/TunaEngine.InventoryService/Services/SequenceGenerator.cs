using MongoDB.Bson;
using MongoDB.Driver;

namespace TunaEngine.InventoryService.Services;

public sealed class SequenceGenerator(IMongoCollection<BsonDocument> sequences)
{
    public async Task<string> NextAsync(string key, string prefix, CancellationToken cancellationToken)
    {
        var result = await sequences.FindOneAndUpdateAsync(
            Builders<BsonDocument>.Filter.Eq("_id", key),
            Builders<BsonDocument>.Update.Inc("value", 1),
            new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = true, ReturnDocument = ReturnDocument.After },
            cancellationToken);
        return $"{prefix}-{result["value"].AsInt64:0000}";
    }
}

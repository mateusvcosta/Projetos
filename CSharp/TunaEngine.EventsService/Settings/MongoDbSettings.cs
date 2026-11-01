namespace TunaEngine.EventsService.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";
    public string ConnectionString { get; set; } = "";
    public string EventsDatabaseName { get; set; } = "Events";
    public string EventsCollectionName { get; set; } = "Events";
}

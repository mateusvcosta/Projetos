namespace TunaEngine.DocumentsService.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";
    public string ConnectionString { get; set; } = "";
    public string DocumentTypesDatabaseName { get; set; } = "Doc_Type";
    public string DocumentsDatabaseName { get; set; } = "Documents";
    public string EventsDatabaseName { get; set; } = "Events";
    public string DocumentTypesCollectionName { get; set; } = "Doc_Type";
    public string DocumentsCollectionName { get; set; } = "Documents";
    public string EventsCollectionName { get; set; } = "Events";
}

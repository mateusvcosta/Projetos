namespace TunaEngine.Api.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "";
    public string DatabaseName { get; set; } = "TunaEngine";
    public string InventoryCollectionName { get; set; } = "inventory";
}

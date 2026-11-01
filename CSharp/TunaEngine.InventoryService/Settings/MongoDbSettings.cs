namespace TunaEngine.InventoryService.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";
    public string ConnectionString { get; set; } = "";
    public string InventoryDatabaseName { get; set; } = "Inventory";
    public string InventoryCollectionName { get; set; } = "Inventory";
}


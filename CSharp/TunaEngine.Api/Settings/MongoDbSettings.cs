namespace TunaEngine.Api.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "";
    public string InventoryDatabaseName { get; set; } = "Inventory";
    public string RolesDatabaseName { get; set; } = "Members";
    public string DocumentTypesDatabaseName { get; set; } = "Doc_Type";
    public string DocumentsDatabaseName { get; set; } = "Documents";
    public string EventsDatabaseName { get; set; } = "Events";
    public string MembersDatabaseName { get; set; } = "Members";
    public string InventoryCollectionName { get; set; } = "Inventory";
    public string RolesCollectionName { get; set; } = "Roles";
    public string DocumentTypesCollectionName { get; set; } = "Doc_Type";
    public string DocumentsCollectionName { get; set; } = "Documents";
    public string EventsCollectionName { get; set; } = "Events";
    public string MembersCollectionName { get; set; } = "Members";
}

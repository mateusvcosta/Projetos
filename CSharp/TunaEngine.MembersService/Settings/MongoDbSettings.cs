namespace TunaEngine.MembersService.Settings;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";
    public string ConnectionString { get; set; } = "";
    public string RolesDatabaseName { get; set; } = "Members";
    public string MembersDatabaseName { get; set; } = "Members";
    public string RolesCollectionName { get; set; } = "Roles";
    public string MembersCollectionName { get; set; } = "Members";
}

namespace TunaEngine.Models;

internal sealed class StockItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Brand { get; set; }
    public float? UnitCost { get; set; }
    public string? Condition { get; set; }
    public string? Function { get; set; }
    public string? Notes { get; set; }
}

internal sealed class StockTable
{
    public string Name { get; set; } = "Itens";
    public List<StockItem> Items { get; } = [];
}

internal sealed class TeamMember
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Role { get; set; } = "Tuno";
    public string TunaName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string StudentNumber { get; set; } = "";
    public string VoiceType { get; set; } = "";
    public string Instrument { get; set; } = "";
    public string Password { get; set; } = "";
}

internal sealed class AgendaEvent
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime? Date { get; set; }
}

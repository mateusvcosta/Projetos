using System.Globalization;

namespace TunaEngine.Views;

internal sealed class StockItem
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
}

internal sealed class StockTable
{
    public string Name { get; set; } = "Itens";
    public List<StockItem> Items { get; } = [];
}

internal sealed class TeamMember
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "Tuno";
}

internal sealed class AgendaEvent
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "Conteúdo de demonstração";
    public DateTime? Date { get; set; }
}

internal static class AppSession
{
    public static readonly CultureInfo Portuguese = CultureInfo.GetCultureInfo("pt-PT");
    private static readonly string[] DayShort = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];
    private static readonly string[] MonthShort =
        ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"];
    private static readonly string[] MonthNames =
        ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
         "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
    private static readonly string[] DayLong =
        ["domingo", "segunda-feira", "terça-feira", "quarta-feira",
         "quinta-feira", "sexta-feira", "sábado"];

    public static DateTime SelectedDate { get; set; } = new(2026, 10, 2);

    public static List<StockTable> Tables { get; } =
    [
        new StockTable
        {
            Name = "Itens",
            Items =
            {
                new StockItem { Name = "Cabo XLR", Quantity = 4 },
                new StockItem { Name = "Microfone", Quantity = 2 },
                new StockItem { Name = "Suporte", Quantity = 3 }
            }
        }
    ];

    public static List<TeamMember> Members { get; } =
    [
        new TeamMember { Name = "João Silva", Role = "Tuno" },
        new TeamMember { Name = "Maria Oliveira", Role = "Tuno" },
        new TeamMember { Name = "Carlos Santos", Role = "Tuno" }
    ];

    public static List<AgendaEvent> Events { get; } =
    [
        new AgendaEvent { Title = "Reunião" },
        new AgendaEvent { Title = "Entrega" },
        new AgendaEvent { Title = "Evento" }
    ];

    public static string ProfileName { get; set; } = "Conteúdo de demonstração";
    public static string ProfileEmail { get; set; } = "Conteúdo de demonstração";
    public static string ProfileRole { get; set; } = "Conteúdo de demonstração";

    public static string Money(decimal? value) =>
        value.HasValue ? value.Value.ToString("C", Portuguese) : "—";

    public static string FormatShortDate(DateTime date) =>
        $"{DayShort[(int)date.DayOfWeek]}, {date.Day} {MonthShort[date.Month - 1]} {date.Year}";

    public static string FormatLongDate(DateTime date) =>
        $"Selecionado: {DayLong[(int)date.DayOfWeek]}, {date.Day} de " +
        $"{MonthNames[date.Month - 1].ToLower(Portuguese)} de {date.Year}";
}

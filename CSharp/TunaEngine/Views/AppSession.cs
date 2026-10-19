using System.Globalization;

namespace TunaEngine.Views;

internal sealed class StockItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int? Quantity { get; set; }
    public decimal? UnitCost { get; set; }
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
}

internal sealed class AgendaEvent
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
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

    public static DateTime SelectedDate { get; set; } = DateTime.Today;

    public static List<StockTable> Tables { get; } = [];
    public static List<TeamMember> Members { get; } = [];
    public static List<AgendaEvent> Events { get; } = [];
    public static string ProfileName { get; set; } = "";
    public static string ProfileEmail { get; set; } = "";
    public static string ProfileRole { get; set; } = "";

    public static string Money(decimal? value) =>
        value.HasValue ? value.Value.ToString("C", Portuguese) : "—";

    public static string FormatShortDate(DateTime date) =>
        $"{DayShort[(int)date.DayOfWeek]}, {date.Day} {MonthShort[date.Month - 1]} {date.Year}";

    public static string FormatLongDate(DateTime date) =>
        $"Selecionado: {DayLong[(int)date.DayOfWeek]}, {date.Day} de " +
        $"{MonthNames[date.Month - 1].ToLower(Portuguese)} de {date.Year}";
}

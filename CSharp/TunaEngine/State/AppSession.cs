using System.Globalization;
using TunaEngine.Models;

namespace TunaEngine.State;

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
    public static string ProfileMemberId { get; set; } = "";
    public static string ProfileEmail { get; set; } = "";
    public static string ProfileRole { get; set; } = "";
    public static string ProfileTunaName { get; set; } = "";
    public static string ProfilePhone { get; set; } = "";
    public static string ProfileStudentNumber { get; set; } = "";
    public static string ProfileVoiceType { get; set; } = "";
    public static string ProfileInstrument { get; set; } = "";

    public static bool IsDirection =>
        string.Equals(ProfileRole, "CAR-0001", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "CAR-0002", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "CAR-00001", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "CAR-00002", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "Magister", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "Presidente", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "Direção", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(ProfileRole, "Direcao", StringComparison.OrdinalIgnoreCase);

    public static string Money(float? value) =>
        value.HasValue ? value.Value.ToString("C", Portuguese) : "—";

    public static string FormatShortDate(DateTime date) =>
        $"{DayShort[(int)date.DayOfWeek]}, {date.Day} {MonthShort[date.Month - 1]} {date.Year}";

    public static string FormatLongDate(DateTime date) =>
        $"Selecionado: {DayLong[(int)date.DayOfWeek]}, {date.Day} de " +
        $"{MonthNames[date.Month - 1].ToLower(Portuguese)} de {date.Year}";
}

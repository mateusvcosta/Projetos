namespace DesktopApp.Maui.Views;

public sealed class CalendarView(bool isWideLayout)
    : RecordSectionView("Calendário", "Datas e atividades da equipa.", "REGISTOS", ["Reunião", "Entrega", "Evento"], isWideLayout);
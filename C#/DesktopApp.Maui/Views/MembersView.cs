namespace DesktopApp.Maui.Views;

public sealed class MembersView(bool isWideLayout)
    : RecordSectionView("Membros", "Pessoas da equipa.", "REGISTOS", ["João", "Maria", "Pedro"], isWideLayout);
namespace DesktopApp.Maui.Views;

public sealed class SettingsView(bool isWideLayout)
    : RecordSectionView("Definições", "Preferências da aplicação.", "REGISTOS", ["Tema", "Notificações", "Segurança"], isWideLayout);
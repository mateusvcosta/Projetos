using TunaEngine.Services;
using TunaEngine.Views;

namespace TunaEngine;

public sealed class App : Application
{
    private readonly TunaEngineApiClient _apiClient;

    public App(TunaEngineApiClient apiClient)
    {
        _apiClient = apiClient;
        var resources = new ResourceDictionary();
        resources.Add("ThemeBackground", new ThemeColor("#F8F7FA", "#202033"));
        resources.Add("ThemeSidebar", new ThemeColor("#FFFFFF", "#1A1A2E"));
        resources.Add("ThemeSurface", new ThemeColor("#FFFFFF", "#2A2A40"));
        resources.Add("ThemeSurfaceDark", new ThemeColor("#F2F1F6", "#252538"));
        resources.Add("ThemeStroke", new ThemeColor("#E3E0E8", "#333355"));
        resources.Add("ThemeAccent", new ThemeColor("#D93657", "#E8526A"));
        resources.Add("ThemeText", new ThemeColor("#28263A", "#F6F6FA"));
        resources.Add("ThemeMuted", new ThemeColor("#777487", "#9999BB"));
        resources.Add("ThemeError", new ThemeColor("#C52D4B", "#F28A9A"));
        resources.Add("ThemeDanger", new ThemeColor("#C52D4B", "#D9364F"));
        Resources = resources;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell(_apiClient))
        {
            Title = "TunaEngine",
            Width = 1280,
            Height = 760
        };

        return window;
    }
}
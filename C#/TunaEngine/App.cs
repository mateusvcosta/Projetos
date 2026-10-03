namespace TunaEngine;

public sealed class App : Application
{
    public App()
    {
        var resources = new ResourceDictionary();
        resources.Add("ThemeBackground", Color.FromArgb("#202033"));
        resources.Add("ThemeSidebar", Color.FromArgb("#1A1A2E"));
        resources.Add("ThemeSurface", Color.FromArgb("#2A2A40"));
        resources.Add("ThemeSurfaceDark", Color.FromArgb("#252538"));
        resources.Add("ThemeStroke", Color.FromArgb("#333355"));
        resources.Add("ThemeAccent", Color.FromArgb("#E8526A"));
        resources.Add("ThemeText", Color.FromArgb("#F6F6FA"));
        resources.Add("ThemeMuted", Color.FromArgb("#9999BB"));
        resources.Add("ThemeError", Color.FromArgb("#F28A9A"));
        Resources = resources;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell())
        {
            Title = "TunaEngine",
            Width = 1280,
            Height = 760
        };

        return window;
    }
}
using TunaEngine.Services;

namespace TunaEngine;

public sealed class AppShell : Shell
{
    public AppShell(TunaEngineApiClient apiClient)
    {
        Title = "TunaEngine";

        var content = new ShellContent
        {
            Title = "Home",
            ContentTemplate = new DataTemplate(() => new MainPage(apiClient)),
            Route = "MainPage"
        };
        var section = new ShellSection();
        section.Items.Add(content);
        var item = new ShellItem();
        item.Items.Add(section);
        Items.Add(item);
    }
}

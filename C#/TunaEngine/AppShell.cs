namespace TunaEngine;

public sealed class AppShell : Shell
{
    public AppShell()
    {
        Title = "TunaEngine";

        var content = new ShellContent
        {
            Title = "Home",
            ContentTemplate = new DataTemplate(typeof(MainPage)),
            Route = "MainPage"
        };
        var section = new ShellSection();
        section.Items.Add(content);
        var item = new ShellItem();
        item.Items.Add(section);
        Items.Add(item);
    }
}

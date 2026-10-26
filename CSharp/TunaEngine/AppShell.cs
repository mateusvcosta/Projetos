using TunaEngine.Services;
using TunaEngine.Views;

namespace TunaEngine;

public sealed class AppShell : Shell
{
    public AppShell(TunaEngineApiClient apiClient)
    {
        Title = "TunaEngine";
        ShowLogin(apiClient);
    }

    private void ShowLogin(TunaEngineApiClient apiClient)
    {
        Items.Clear();
        var login = new ShellContent
        {
            Title = "Entrar",
            ContentTemplate = new DataTemplate(() => new LoginView(apiClient, response =>
            {
                AppSession.ProfileName = response.Name;
                AppSession.ProfileEmail = response.Email;
                ShowMain(apiClient);
            })),
            Route = "Login"
        };
        var section = new ShellSection();
        section.Items.Add(login);
        var item = new ShellItem();
        item.Items.Add(section);
        Items.Add(item);
    }

    private void ShowMain(TunaEngineApiClient apiClient)
    {
        Items.Clear();
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

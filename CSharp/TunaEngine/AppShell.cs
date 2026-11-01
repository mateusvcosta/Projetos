using TunaEngine.Services;
using TunaEngine.Views;
using TunaEngine.Views.Popups;

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
                AppSession.ProfileMemberId = response.Id;
                AppSession.ProfileName = response.Name;
                AppSession.ProfileEmail = response.Email;
                AppSession.ProfileRole = response.RoleId ?? "";
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
            ContentTemplate = new DataTemplate(() => new MainPage(apiClient, () => SignOut(apiClient))),
            Route = "MainPage"
        };
        var section = new ShellSection();
        section.Items.Add(content);
        var item = new ShellItem();
        item.Items.Add(section);
        Items.Add(item);
    }

    private async Task SignOut(TunaEngineApiClient apiClient)
    {
        if (CurrentPage is not ContentPage page)
            return;

        var popup = new ConfirmationPopup(
            "Terminar sessão",
            "Tem a certeza de que pretende terminar a sessão?",
            "Terminar");
        var confirmed = await ModalOverlay.ShowAsync(
            page, popup, popup.Completion, popup.Cancel);
        if (!confirmed)
            return;

        AppSession.ProfileMemberId = "";
        AppSession.ProfileName = "";
        AppSession.ProfileEmail = "";
        AppSession.ProfileRole = "";
        ShowLogin(apiClient);
    }
}

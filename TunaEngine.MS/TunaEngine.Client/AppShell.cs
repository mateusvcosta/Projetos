using TunaEngine.Services;
using TunaEngine.Views;

namespace TunaEngine;

public sealed class AppShell : Shell
{
    public AppShell(TunaEngineApiClient apiClient)
    {
        Title = "TunaEngine (Microservices)";
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
                return FinishLoginAsync(apiClient, response);
            })),
            Route = "Login"
        };
        var section = new ShellSection();
        section.Items.Add(login);
        var item = new ShellItem();
        item.Items.Add(section);
        Items.Add(item);
    }

    private async Task FinishLoginAsync(TunaEngineApiClient apiClient, ApiLoginResponse response)
    {
        apiClient.SetAccessToken(response.AccessToken);

        ApiMemberProfile profile;
        try
        {
            profile = await apiClient.GetAsync<ApiMemberProfile>("api/members/me");
        }
        catch (HttpRequestException exception) when (
            exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            profile = await apiClient.PutAsync<ApiMemberProfile>(
                "api/members/me",
                new ApiMemberProfileRequest { TunaName = "Tuna" });
        }

        AppSession.ProfileMemberId = profile.Id;
        AppSession.ProfileName = profile.Name;
        AppSession.ProfileEmail = profile.Email;
        AppSession.ProfileRole = profile.Role;
        AppSession.ProfileTunaName = profile.TunaName;
        AppSession.ProfilePhone = profile.Phone ?? "";
        AppSession.ProfileStudentNumber = profile.StudentNumber ?? "";
        AppSession.ProfileVoiceType = profile.VoiceType ?? "";
        AppSession.ProfileInstrument = profile.Instrument ?? "";

        ShowMain(apiClient);
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

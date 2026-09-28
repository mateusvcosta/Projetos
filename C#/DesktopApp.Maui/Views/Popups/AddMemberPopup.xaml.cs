using CommunityToolkit.Maui.Views;

namespace DesktopApp.Maui.Views.Popups;

public partial class AddMemberPopup : Popup
{
    public MemberDraft? Member { get; private set; }

    public AddMemberPopup()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var role = RoleEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
        {
            ErrorLabel.Text = "Introduza o nome e o cargo do membro.";
            ErrorLabel.IsVisible = true;
            return;
        }

        Member = new MemberDraft(name, role);
        await CloseAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync();
    }
}

public sealed record MemberDraft(string Name, string Role);
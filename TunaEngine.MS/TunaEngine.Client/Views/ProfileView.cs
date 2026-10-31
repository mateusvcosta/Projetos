using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;

namespace TunaEngine.Views;

public sealed class ProfileView : ContentView
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Label _nameValue = CreateValueLabel();
    private readonly Label _emailValue = CreateValueLabel();
    private readonly Label _roleValue = CreateValueLabel();
    private readonly Label _heroTitle = ViewTheme.TextLabel("", 26, true, ViewTheme.Text);
    private readonly Label _heroDescription = ViewTheme.TextLabel("", 14, color: ViewTheme.Muted);
    private readonly Label _statusLabel = ViewTheme.TextLabel("", 12, color: ViewTheme.Muted);

    public ProfileView(TunaEngineApiClient apiClient)
    {
        _apiClient = apiClient;
        var content = new VerticalStackLayout { Spacing = 24 };
        content.Children.Add(CreateHeroCard());

        var summaryGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(1.2, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star))
            },
            ColumnSpacing = 24
        };

        summaryGrid.Add(CreateDetailsCard(), 0, 0);
        summaryGrid.Add(CreateActivityCard(), 1, 0);
        content.Children.Add(summaryGrid);

        Content = content;
        UpdateLabels();
        _ = LoadMemberAsync();
    }

    private async Task EditProfile()
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var tunaName = await shell.DisplayPromptAsync(
            "Editar perfil", "Nome da tuna:", "Guardar", "Cancelar",
            initialValue: AppSession.ProfileTunaName);
        if (string.IsNullOrWhiteSpace(tunaName))
        {
            return;
        }

        try
        {
            var profile = await _apiClient.PutAsync<ApiMemberProfile>(
                "api/members/me",
                new ApiMemberProfileRequest
                {
                    TunaName = tunaName.Trim(),
                    Phone = AppSession.ProfilePhone,
                    StudentNumber = AppSession.ProfileStudentNumber,
                    VoiceType = AppSession.ProfileVoiceType,
                    Instrument = AppSession.ProfileInstrument
                });
            AppSession.ProfileTunaName = profile.TunaName;
            _statusLabel.Text = "Perfil guardado";
            UpdateLabels();
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            await shell.DisplayAlertAsync("Perfil", "Não foi possível guardar o perfil. " + exception.Message, "OK");
        }
    }

    private Border CreateHeroCard()
    {
        var avatar = new Border
        {
            WidthRequest = 96,
            HeightRequest = 96,
            BackgroundColor = ViewTheme.Accent,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 48 }
        };

        var heroText = new VerticalStackLayout { Spacing = 6 };
        heroText.Children.Add(_heroTitle);
        heroText.Children.Add(_heroDescription);

        var chips = new HorizontalStackLayout { Spacing = 8, Margin = new Thickness(0, 1, 0, 0) };
        chips.Children.Add(CreateStatusChip("Membro", "#1A3A2A", "#4ADE80"));
        chips.Children.Add(_statusLabel);
        heroText.Children.Add(chips);

        var heroLayout = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 32,
            Padding = new Thickness(32)
        };

        heroLayout.Add(avatar, 0, 0);
        heroLayout.Add(heroText, 1, 0);

        var editButton = ViewTheme.Action("Editar Perfil", EditProfile, true);
        editButton.WidthRequest = 120;
        editButton.HeightRequest = 40;
        editButton.FontSize = 13;
        editButton.CornerRadius = 8;
        editButton.Padding = new Thickness(0);
        editButton.BackgroundColor = ViewTheme.Accent;
        editButton.TextColor = Colors.White;
        editButton.HorizontalOptions = LayoutOptions.End;
        Grid.SetColumn(editButton, 2);
        heroLayout.Add(editButton);

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = heroLayout
        };
    }

    private async Task LoadMemberAsync()
    {
        try
        {
            var member = await _apiClient.GetAsync<ApiMemberProfile>("api/members/me");
            AppSession.ProfileName = member.Name;
            AppSession.ProfileEmail = member.Email;
            AppSession.ProfileTunaName = member.TunaName;
            AppSession.ProfilePhone = member.Phone ?? "";
            AppSession.ProfileStudentNumber = member.StudentNumber ?? "";
            AppSession.ProfileVoiceType = member.VoiceType ?? "";
            AppSession.ProfileInstrument = member.Instrument ?? "";
            AppSession.ProfileRole = member.Role;

            UpdateLabels();
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            _statusLabel.Text = "Não foi possível carregar os dados do perfil.";
        }
    }

    private void UpdateLabels()
    {
        _nameValue.Text = ValueOrDash(AppSession.ProfileName);
        _emailValue.Text = ValueOrDash(AppSession.ProfileEmail);
        _roleValue.Text = ValueOrDash(AppSession.ProfileRole);
        _heroTitle.Text = ValueOrDash(AppSession.ProfileName);
        _heroDescription.Text = string.IsNullOrWhiteSpace(AppSession.ProfileTunaName)
            ? "Perfil do membro"
            : AppSession.ProfileTunaName;
        if (string.IsNullOrWhiteSpace(_statusLabel.Text))
        {
            _statusLabel.Text = "Dados da base de dados";
        }
    }

    private static Label CreateValueLabel() =>
        ViewTheme.TextLabel("", 14, false, Color.FromArgb("#CCCCDD"));

    private static string ValueOrDash(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "—" : value;

    private Border CreateDetailsCard()
    {
        var content = new VerticalStackLayout { Spacing = 0 };
        content.Children.Add(CreateSectionHeader("Informação Pessoal"));
        content.Children.Add(CreateFieldRow("Nome", _nameValue));
        content.Children.Add(ViewTheme.Divider());
        content.Children.Add(CreateFieldRow("Email", _emailValue));
        content.Children.Add(ViewTheme.Divider());
        content.Children.Add(CreateFieldRow("Cargo na equipa", _roleValue));

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = content
        };
    }

    private Border CreateActivityCard()
    {
        var content = new VerticalStackLayout { Spacing = 0 };
        content.Children.Add(CreateSectionHeader("Atividade Recente"));

        var empty = new VerticalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(0, 24),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HeightRequest = 200
        };

        var icon = new Border
        {
            WidthRequest = 40,
            HeightRequest = 40,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Padding = new Thickness(10),
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Center
        };
        icon.Content = new Label
        {
            Text = "◌",
            FontSize = 18,
            TextColor = ViewTheme.Muted,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        empty.Children.Add(icon);
        empty.Children.Add(ViewTheme.TextLabel("Sem atividade recente", 14, color: Color.FromArgb("#A3A3B8")));
        empty.Children.Add(ViewTheme.TextLabel("Conteúdo de demonstração", 12, color: Color.FromArgb("#444466")));
        content.Children.Add(empty);

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = content
        };
    }

    private static Grid CreateSectionHeader(string title)
    {
        var header = new Grid
        {
            Padding = new Thickness(20, 0),
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent
        };

        var label = ViewTheme.TextLabel(title, 11, true, ViewTheme.Muted);
        label.TextTransform = TextTransform.Uppercase;
        header.Add(label);
        return header;
    }

    private static Grid CreateFieldRow(string labelText, Label valueLabel)
    {
        var row = new Grid
        {
            Padding = new Thickness(20, 0),
            HeightRequest = 56,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            BackgroundColor = Colors.Transparent
        };

        var label = ViewTheme.TextLabel(labelText, 14, false, ViewTheme.Muted);
        valueLabel.FontSize = 14;
        valueLabel.TextColor = Color.FromArgb("#CCCCDD");
        valueLabel.HorizontalTextAlignment = TextAlignment.End;
        valueLabel.VerticalTextAlignment = TextAlignment.Center;

        row.Add(label, 0, 0);
        row.Add(valueLabel, 1, 0);
        return row;
    }

    private static Border CreateStatusChip(string text, string bg, string fg)
    {
        var label = ViewTheme.TextLabel(text, 11, true, Color.FromArgb(fg));
        label.VerticalTextAlignment = TextAlignment.Center;
        label.HorizontalTextAlignment = TextAlignment.Center;

        return new Border
        {
            BackgroundColor = Color.FromArgb(bg),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            Padding = new Thickness(12, 6),
            Content = label,
            HeightRequest = 26
        };
    }
}

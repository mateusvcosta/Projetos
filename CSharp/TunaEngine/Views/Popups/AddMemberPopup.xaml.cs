using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class AddMemberPopup : ContentView
{
    private readonly TaskCompletionSource<MemberDraft?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Entry _nameEntry;
    private readonly Picker _rolePicker;
    private readonly Entry _tunaNameEntry;
    private readonly Entry _emailEntry;
    private readonly Entry _phoneEntry;
    private readonly Entry _studentNumberEntry;
    private readonly Entry _voiceTypeEntry;
    private readonly Entry _instrumentEntry;
    private readonly Entry _passwordEntry;
    private readonly Label _errorLabel;
    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;

    public Task<MemberDraft?> Completion => _completion.Task;

    internal AddMemberPopup(TeamMember? member = null, IEnumerable<string>? roles = null)
    {
        BackgroundColor = Colors.Transparent;
        Padding = 0;

        _nameEntry = CreateEntry("Nome do membro", 80);
        var roleOptions = (roles ?? [])
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        if (member is not null
            && !roleOptions.Contains(member.Role, StringComparer.CurrentCultureIgnoreCase))
        {
            roleOptions.Insert(0, member.Role);
        }

        _rolePicker = new Picker
        {
            Title = "Selecionar cargo",
            TitleColor = ViewTheme.Muted,
            TextColor = ViewTheme.Text,
            BackgroundColor = Colors.Transparent,
            FontSize = 13,
            ItemsSource = roleOptions,
            SelectedItem = member?.Role ?? roleOptions.FirstOrDefault(),
            VerticalOptions = LayoutOptions.Fill
        };
        _tunaNameEntry = CreateEntry("Nome da tuna", 100);
        _emailEntry = CreateEntry("Email", 160);
        _phoneEntry = CreateEntry("Telefone", 30);
        _studentNumberEntry = CreateEntry("Número de estudante", 30);
        _voiceTypeEntry = CreateEntry("Tipo de voz", 60);
        _instrumentEntry = CreateEntry("Instrumento", 80);
        _passwordEntry = CreateEntry("Palavra-passe", 100);
        _passwordEntry.IsPassword = true;

        _nameEntry.Text = member?.Name;
        _tunaNameEntry.Text = member?.TunaName;
        _emailEntry.Text = member?.Email;
        _phoneEntry.Text = member?.Phone;
        _studentNumberEntry.Text = member?.StudentNumber;
        _voiceTypeEntry.Text = member?.VoiceType;
        _instrumentEntry.Text = member?.Instrument;
        _passwordEntry.Text = member?.Password;
        _errorLabel = new Label
        {
            IsVisible = false,
            FontSize = 13,
            TextColor = ViewTheme.Error
        };

        var cancelButton = CreateButton("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancelButton.Clicked += (_, _) => Cancel();
        var saveButton = CreateButton("Adicionar", Colors.White, ViewTheme.Accent);
        saveButton.Clicked += (_, _) => Save();

        _titleLabel = new Label
        {
            Text = "Novo membro",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = ViewTheme.Text
        };
        _descriptionLabel = new Label
        {
            Text = "Adicione um membro à tabela Tunos.",
            FontSize = 14,
            TextColor = ViewTheme.Muted
        };

        var fields = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowSpacing = 10,
            ColumnSpacing = 10
        };
        View[] entries =
        {
            _nameEntry, CreateRoleControl(), _tunaNameEntry, _emailEntry,
            _phoneEntry, _studentNumberEntry, _voiceTypeEntry, _instrumentEntry,
            _passwordEntry
        };
        for (var index = 0; index < entries.Length; index++)
        {
            fields.Add(entries[index], index % 2, index / 2);
        }

        var form = new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                new VerticalStackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        _titleLabel,
                        _descriptionLabel
                    }
                },
                new VerticalStackLayout
                {
                    Spacing = 10,
                    Children = { fields, _errorLabel }
                },
                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 10,
                    Children = { cancelButton, saveButton }
                }
            }
        };
        Grid.SetColumn(saveButton, 1);

        Content = new Border
        {
            WidthRequest = 520,
            MaximumWidthRequest = 600,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new ScrollView
            {
                MaximumHeightRequest = 700,
                Content = form
            }
        };

        if (member is not null)
        {
            _titleLabel.Text = "Editar membro";
            _descriptionLabel.Text = "Atualize os dados do membro.";
        }
    }

    private static Entry CreateEntry(string placeholder, int maxLength) =>
        new()
        {
            Placeholder = placeholder,
            MaxLength = maxLength,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark
        };

    private static StateButton CreateButton(string text, Color textColor, Color backgroundColor)
    {
        var button = new StateButton
        {
            Text = text,
            TextColor = textColor,
            BackgroundColor = backgroundColor,
            CornerRadius = 10
        };
        SemanticProperties.SetDescription(button, text);
        return button;
    }

    private void Save()
    {
        var name = _nameEntry.Text?.Trim();
        var role = (_rolePicker.SelectedItem as string)?.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
        {
            _errorLabel.Text = "Introduza o nome e o cargo do membro.";
            _errorLabel.IsVisible = true;
            return;
        }

        _completion.TrySetResult(new MemberDraft(
            name,
            role,
            _tunaNameEntry.Text?.Trim() ?? "",
            _emailEntry.Text?.Trim() ?? "",
            _phoneEntry.Text?.Trim() ?? "",
            _studentNumberEntry.Text?.Trim() ?? "",
            _voiceTypeEntry.Text?.Trim() ?? "",
            _instrumentEntry.Text?.Trim() ?? "",
            _passwordEntry.Text ?? ""));
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));

    private View CreateRoleControl() =>
        new Border
        {
            HeightRequest = 38,
            BackgroundColor = ViewTheme.SurfaceDark,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 7 },
            Padding = new Thickness(8, 0),
            Content = _rolePicker
        };
}

public sealed record MemberDraft(
    string Name,
    string Role,
    string TunaName,
    string Email,
    string Phone,
    string StudentNumber,
    string VoiceType,
    string Instrument,
    string Password);
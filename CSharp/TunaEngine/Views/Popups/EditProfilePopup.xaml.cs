using Microsoft.Maui.Controls.Shapes;

namespace TunaEngine.Views.Popups;

public sealed class EditProfilePopup : ContentView
{
    private readonly TaskCompletionSource<ProfileDraft?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Entry _name;
    private readonly Entry _email;
    private readonly Entry _tunaName;
    private readonly Entry _phone;
    private readonly Entry _studentNumber;
    private readonly Entry _voiceType;
    private readonly Entry _instrument;
    private readonly Entry _password;
    private readonly Label _error;

    public Task<ProfileDraft?> Completion => _completion.Task;

    public EditProfilePopup()
    {
        _name = Entry("Nome", AppSession.ProfileName, 80);
        _email = Entry("Email", AppSession.ProfileEmail, 160);
        _email.Keyboard = Keyboard.Email;
        _tunaName = Entry("Nome da tuna", AppSession.ProfileTunaName, 100);
        _phone = Entry("Telefone", AppSession.ProfilePhone, 30);
        _studentNumber = Entry("Número de estudante", AppSession.ProfileStudentNumber, 30);
        _voiceType = Entry("Tipo de voz", AppSession.ProfileVoiceType, 60);
        _instrument = Entry("Instrumento", AppSession.ProfileInstrument, 80);
        _password = Entry("Nova palavra-passe (opcional)", "", 100);
        _password.IsPassword = true;
        _error = new Label { IsVisible = false, FontSize = 13, TextColor = ViewTheme.Error };

        var cancel = Button("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancel.Clicked += (_, _) => Cancel();
        var save = Button("Guardar", Colors.White, ViewTheme.Accent);
        save.Clicked += (_, _) => Save();
        var buttons = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children = { cancel, save }
        };
        Grid.SetColumn(save, 1);

        Content = new Border
        {
            WidthRequest = 460,
            MaximumWidthRequest = 560,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new ScrollView
            {
                MaximumHeightRequest = 700,
                Content = new VerticalStackLayout
                {
                    Spacing = 12,
                    Children =
                    {
                        ViewTheme.TextLabel("Editar perfil", 22, true),
                        ViewTheme.TextLabel("Atualize os seus dados pessoais.", 14, color: ViewTheme.Muted),
                        _name, _email, _tunaName, _phone, _studentNumber,
                        _voiceType, _instrument, _password, _error, buttons
                    }
                }
            }
        };
    }

    private static Entry Entry(string placeholder, string? value, int maxLength) => new()
    {
        Placeholder = placeholder,
        Text = value,
        MaxLength = maxLength,
        TextColor = ViewTheme.Text,
        PlaceholderColor = ViewTheme.Muted,
        BackgroundColor = ViewTheme.SurfaceDark
    };

    private static StateButton Button(string text, Color textColor, Color background) => new()
    {
        Text = text,
        TextColor = textColor,
        BackgroundColor = background,
        CornerRadius = 10
    };

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_name.Text) || string.IsNullOrWhiteSpace(_email.Text))
        {
            _error.Text = "Nome e email são obrigatórios.";
            _error.IsVisible = true;
            return;
        }

        if (!string.IsNullOrWhiteSpace(_password.Text) && _password.Text.Length < 8)
        {
            _error.Text = "A palavra-passe deve ter pelo menos 8 caracteres.";
            _error.IsVisible = true;
            return;
        }

        _completion.TrySetResult(new ProfileDraft(
            _name.Text.Trim(), _email.Text.Trim(), _tunaName.Text?.Trim() ?? "",
            _phone.Text?.Trim() ?? "", _studentNumber.Text?.Trim() ?? "",
            _voiceType.Text?.Trim() ?? "", _instrument.Text?.Trim() ?? "",
            _password.Text ?? ""));
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));
}

public sealed record ProfileDraft(
    string Name,
    string Email,
    string TunaName,
    string Phone,
    string StudentNumber,
    string VoiceType,
    string Instrument,
    string Password);

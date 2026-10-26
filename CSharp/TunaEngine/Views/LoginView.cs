using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;

namespace TunaEngine.Views;

public sealed class LoginView : ContentPage
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Action<ApiLoginResponse> _authenticated;
    private readonly Entry _emailEntry;
    private readonly Entry _passwordEntry;
    private readonly Button _showPasswordButton;
    private readonly Button _loginButton;
    private readonly Label _errorLabel;
    private bool _isPasswordVisible;
    private bool _isSigningIn;

    public LoginView(
        TunaEngineApiClient apiClient,
        Action<ApiLoginResponse> authenticated)
    {
        _apiClient = apiClient;
        _authenticated = authenticated;
        BackgroundColor = ViewTheme.Background;
        Shell.SetNavBarIsVisible(this, false);
        Title = "Entrar";

        _emailEntry = CreateEntry("Introduza o seu email");
        _emailEntry.Keyboard = Keyboard.Email;
        _emailEntry.ReturnType = ReturnType.Next;

        _passwordEntry = CreateEntry("Introduza a sua palavra-passe");
        _passwordEntry.IsPassword = true;
        _passwordEntry.ReturnType = ReturnType.Done;
        _passwordEntry.Completed += async (_, _) => await SignInAsync();

        _showPasswordButton = new Button
        {
            Text = "Mostrar",
            FontSize = 11,
            TextColor = ViewTheme.Text,
            BackgroundColor = Colors.Transparent,
            Padding = new Thickness(4, 0),
            MinimumWidthRequest = 54,
            MinimumHeightRequest = 0
        };
        _showPasswordButton.Clicked += (_, _) => TogglePasswordVisibility();

        _errorLabel = new Label
        {
            IsVisible = false,
            FontSize = 12,
            TextColor = ViewTheme.Error,
            LineBreakMode = LineBreakMode.WordWrap
        };

        _loginButton = new Button
        {
            Text = "Entrar",
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = ViewTheme.Accent,
            CornerRadius = 8,
            HeightRequest = 44
        };
        _loginButton.Clicked += async (_, _) => await SignInAsync();

        Content = CreateLayout();
    }

    private View CreateLayout()
    {
        var header = new Grid
        {
            HeightRequest = 72,
            Padding = new Thickness(48, 0),
            BackgroundColor = ViewTheme.Sidebar,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(22)),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8
        };
        header.Add(new Border
        {
            WidthRequest = 22,
            HeightRequest = 22,
            BackgroundColor = ViewTheme.Accent,
            Stroke = Colors.Transparent,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 5 },
            Content = new Label
            {
                Text = "T",
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }
        }, 0, 0);
        header.Add(ViewTheme.TextLabel("TunaEngine", 13, true), 1, 0);

        var card = new Border
        {
            WidthRequest = 326,
            MaximumWidthRequest = 420,
            Padding = new Thickness(26, 25),
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    ViewTheme.TextLabel("Bem-vindo!", 22, true),
                    CreateDescription(),
                    CreateFieldLabel("Email"),
                    CreateEntryBorder(_emailEntry),
                    CreateFieldLabel("Palavra-passe"),
                    CreatePasswordEntryBorder(),
                    _errorLabel,
                    CreateForgotPasswordButton(),
                    _loginButton
                }
            }
        };

        var accessArea = new Grid
        {
            Padding = new Thickness(24, 64, 24, 112),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        };
        accessArea.Add(card);

        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(new GridLength(72)),
                new RowDefinition(GridLength.Star)
            }
        };
        root.Add(header, 0, 0);
        root.Add(accessArea, 0, 1);
        return root;
    }

    private Label CreateDescription()
    {
        var label = ViewTheme.TextLabel("Entre na sua conta para continuar.", 11, color: ViewTheme.Muted);
        label.Margin = new Thickness(0, 5, 0, 22);
        return label;
    }

    private Label CreateFieldLabel(string text)
    {
        var label = ViewTheme.TextLabel(text, 11, color: ViewTheme.Text);
        label.Margin = new Thickness(0, 0, 0, 6);
        return label;
    }

    private Border CreateEntryBorder(View content)
    {
        var border = new Border
        {
            HeightRequest = 38,
            Margin = new Thickness(0, 0, 0, 14),
            Padding = new Thickness(10, 0),
            BackgroundColor = ViewTheme.SurfaceDark,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 7 },
            Content = content
        };
        return border;
    }

    private Border CreatePasswordEntryBorder()
    {
        var content = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        content.Add(_passwordEntry, 0, 0);
        content.Add(_showPasswordButton, 1, 0);
        return CreateEntryBorder(content);
    }

    private Button CreateForgotPasswordButton()
    {
        var button = new Button
        {
            Text = "Esqueceu-se da palavra-passe?",
            FontSize = 11,
            TextColor = ViewTheme.Accent,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.End,
            Padding = new Thickness(0),
            HeightRequest = 32
        };
        button.Clicked += async (_, _) =>
            await DisplayAlertAsync(
                "Recuperar palavra-passe",
                "Contacte um administrador para recuperar o acesso à sua conta.",
                "OK");
        return button;
    }

    private static Entry CreateEntry(string placeholder) =>
        new()
        {
            Placeholder = placeholder,
            PlaceholderColor = ViewTheme.Muted,
            TextColor = ViewTheme.Text,
            BackgroundColor = Colors.Transparent,
            FontSize = 12,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing
        };

    private void TogglePasswordVisibility()
    {
        _isPasswordVisible = !_isPasswordVisible;
        _passwordEntry.IsPassword = !_isPasswordVisible;
        _showPasswordButton.Text = _isPasswordVisible ? "Ocultar" : "Mostrar";
    }

    private async Task SignInAsync()
    {
        if (_isSigningIn)
        {
            return;
        }

        var email = _emailEntry.Text?.Trim();
        var password = _passwordEntry.Text;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Introduza o email e a palavra-passe.");
            return;
        }

        _isSigningIn = true;
        _loginButton.IsEnabled = false;
        _loginButton.Text = "A entrar...";
        _errorLabel.IsVisible = false;

        try
        {
            var response = await _apiClient.PostAsync<ApiLoginResponse>(
                "api/members/login",
                new ApiLoginRequest { Email = email, Password = password });
            _authenticated(response);
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            ShowError("Email ou palavra-passe inválidos.");
        }
        finally
        {
            _isSigningIn = false;
            _loginButton.IsEnabled = true;
            _loginButton.Text = "Entrar";
        }
    }

    private void ShowError(string message)
    {
        _errorLabel.Text = message;
        _errorLabel.IsVisible = true;
    }
}

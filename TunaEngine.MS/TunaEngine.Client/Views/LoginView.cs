using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;

namespace TunaEngine.Views;

public sealed class LoginView : ContentPage
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Func<ApiLoginResponse, Task> _authenticated;
    private readonly Entry _emailEntry;
    private readonly Entry _passwordEntry;
    private readonly Border _rememberMeBox;
    private readonly Button _showPasswordButton;
    private readonly Button _loginButton;
    private readonly Button _registerButton;
    private readonly Label _errorLabel;
    private bool _isPasswordVisible;
    private bool _isSigningIn;

    public LoginView(
        TunaEngineApiClient apiClient,
        Func<ApiLoginResponse, Task> authenticated)
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

        var rememberMeEnabled = Preferences.Get("RememberLoginEmail", false);
        _rememberMeBox = new Border
        {
            WidthRequest = 20,
            HeightRequest = 20,
            BackgroundColor = rememberMeEnabled ? ViewTheme.Accent : Colors.Transparent,
            Stroke = ViewTheme.Accent,
            StrokeThickness = 1.5,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = "✓",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                IsVisible = rememberMeEnabled
            }
        };
        _emailEntry.Text = rememberMeEnabled
            ? Preferences.Get("LoginEmail", "")
            : "";

        _showPasswordButton = new Button
        {
            AutomationId = "LoginShowPasswordButton",
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
            CornerRadius = 11,
            HeightRequest = 48,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 10, 0, 0)
        };
        _loginButton.Clicked += async (_, _) => await SignInAsync();

        _registerButton = new Button
        {
            Text = "Criar conta",
            FontSize = 12,
            TextColor = ViewTheme.Accent,
            BackgroundColor = Colors.Transparent,
            HeightRequest = 40,
            Margin = new Thickness(0, 4, 0, 0)
        };
        _registerButton.Clicked += async (_, _) => await RegisterAsync();

        Content = CreateLayout();
    }

    private View CreateLayout()
    {
        var header = new Grid
        {
            HeightRequest = 72,
            Padding = new Thickness(24, 0),
            BackgroundColor = ViewTheme.Background,
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
        header.Add(ViewTheme.TextLabel("TunaEngine · Microservices", 13, true), 1, 0);

        var card = new Border
        {
            WidthRequest = 326,
            MaximumWidthRequest = 420,
            Padding = new Thickness(0, 8),
            BackgroundColor = Colors.Transparent,
            Stroke = Colors.Transparent,
            StrokeThickness = 0,
            VerticalOptions = LayoutOptions.Center,
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
                    CreateLoginOptionsRow(),
                    _loginButton,
                    _registerButton
                }
            }
        };

        var accessArea = new Grid
        {
            Padding = new Thickness(24, 24),
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
        label.Margin = new Thickness(0, 5, 0, 30);
        return label;
    }

    private Label CreateFieldLabel(string text)
    {
        var label = ViewTheme.TextLabel(text, 11, color: ViewTheme.Text);
        label.Margin = new Thickness(0, 0, 0, 8);
        return label;
    }

    private Border CreateEntryBorder(View content)
    {
        var border = new Border
        {
            HeightRequest = 46,
            Margin = new Thickness(0, 0, 0, 20),
            Padding = new Thickness(12, 0),
            BackgroundColor = ViewTheme.SurfaceDark,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 11 },
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
            AutomationId = "LoginForgotPasswordButton",
            Text = "Esqueceu-se da palavra-passe?",
            FontSize = 11,
            TextColor = ViewTheme.Accent,
            BackgroundColor = Colors.Transparent,
            Padding = new Thickness(0),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            HeightRequest = 30,
            Margin = 0
        };
        button.Clicked += async (_, _) =>
            await DisplayAlertAsync(
                "Recuperar palavra-passe",
                "Contacte um administrador para recuperar o acesso à sua conta.",
                "OK");
        return button;
    }

    private Grid CreateLoginOptionsRow()
    {
        var row = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 0,
            HeightRequest = 30,
            Margin = new Thickness(0, -8, 0, 0)
        };

        var rememberMe = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center
        };
        rememberMe.Add(_rememberMeBox, 0, 0);
        var rememberMeLabel = ViewTheme.TextLabel("Manter sessão iniciada", 11, color: ViewTheme.Muted);
        rememberMeLabel.VerticalOptions = LayoutOptions.Center;
        rememberMe.Add(rememberMeLabel, 1, 0);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (_, _) => ToggleRememberMe();
        rememberMe.GestureRecognizers.Add(tapGesture);
        row.Add(rememberMe, 0, 0);
        row.Add(CreateForgotPasswordButton(), 1, 0);
        return row;
    }

    private bool IsRememberMeEnabled() =>
        _rememberMeBox.Content is Label { IsVisible: true };

    private void ToggleRememberMe()
    {
        var enabled = !IsRememberMeEnabled();
        _rememberMeBox.BackgroundColor = enabled ? ViewTheme.Accent : Colors.Transparent;
        if (_rememberMeBox.Content is Label checkmark)
        {
            checkmark.IsVisible = enabled;
        }

        if (!enabled)
        {
            Preferences.Remove("LoginEmail");
        }
    }

    private static Entry CreateEntry(string placeholder) =>
        new()
        {
            AutomationId = "LoginEntry",
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
        _registerButton.IsEnabled = false;
        _loginButton.Text = "A entrar...";
        _errorLabel.IsVisible = false;

        try
        {
            var response = await _apiClient.PostAsync<ApiLoginResponse>(
                "api/auth/login",
                new ApiLoginRequest { Email = email, Password = password });

            if (IsRememberMeEnabled())
            {
                Preferences.Set("LoginEmail", email);
                Preferences.Set("RememberLoginEmail", true);
            }
            else
            {
                Preferences.Remove("LoginEmail");
                Preferences.Set("RememberLoginEmail", false);
            }

            await _authenticated(response);
        }
        catch (HttpRequestException exception) when (
            exception.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ShowError("Email ou palavra-passe inválidos.");
        }
        catch (HttpRequestException)
        {
            ShowError("Não foi possível contactar o servidor. Verifique a ligação e tente novamente.");
        }
        catch (TaskCanceledException)
        {
            ShowError("O servidor demorou demasiado tempo a responder. Tente novamente.");
        }
        catch (Exception exception) when (
            exception is JsonException or InvalidOperationException)
        {
            ShowError("O servidor devolveu uma resposta inválida. Tente novamente mais tarde.");
        }
        finally
        {
            _isSigningIn = false;
            _loginButton.IsEnabled = true;
            _registerButton.IsEnabled = true;
            _loginButton.Text = "Entrar";
        }
    }

    private async Task RegisterAsync()
    {
        if (_isSigningIn)
        {
            return;
        }

        var email = _emailEntry.Text?.Trim();
        var password = _passwordEntry.Text;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Introduza o email e uma palavra-passe com pelo menos 8 caracteres.");
            return;
        }

        var name = await DisplayPromptAsync(
            "Criar conta", "Nome do membro:", "Continuar", "Cancelar", maxLength: 100);
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        _isSigningIn = true;
        _loginButton.IsEnabled = false;
        _registerButton.IsEnabled = false;
        _loginButton.Text = "A criar conta...";
        _errorLabel.IsVisible = false;

        try
        {
            await _apiClient.PostAsync<ApiRegisteredUser>(
                "api/auth/register",
                new ApiRegisterRequest { Name = name.Trim(), Email = email, Password = password });

            var response = await _apiClient.PostAsync<ApiLoginResponse>(
                "api/auth/login",
                new ApiLoginRequest { Email = email, Password = password });
            Preferences.Set("LoginEmail", email);
            await _authenticated(response);
        }
        catch (HttpRequestException exception) when (
            exception.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            ShowError("Este email já tem conta. Entre com a palavra-passe ou escolha outro email.");
        }
        catch (HttpRequestException exception) when (
            exception.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            ShowError("Verifique o email e confirme que a palavra-passe tem pelo menos 8 caracteres.");
        }
        catch (HttpRequestException)
        {
            ShowError("Não foi possível criar a conta. Verifique se os serviços estão iniciados.");
        }
        catch (TaskCanceledException)
        {
            ShowError("O servidor demorou demasiado tempo a responder. Tente novamente.");
        }
        finally
        {
            _isSigningIn = false;
            _loginButton.IsEnabled = true;
            _registerButton.IsEnabled = true;
            _loginButton.Text = "Entrar";
        }
    }

    private void ShowError(string message)
    {
        _errorLabel.Text = message;
        _errorLabel.IsVisible = true;
    }
}

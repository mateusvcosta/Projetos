using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class AddEventPopup : ContentView
{
    private readonly TaskCompletionSource<EventDraft?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Entry _titleEntry;
    private readonly Entry _locationEntry;
    private readonly Entry _globalIdLocalEntry;
    private readonly Label _errorLabel;

    public Task<EventDraft?> Completion => _completion.Task;

    public AddEventPopup()
    {
        BackgroundColor = Colors.Transparent;
        Padding = 0;

        _titleEntry = CreateEntry("Nome do evento", 100);
        _locationEntry = CreateEntry("Localização ou detalhes", 250);
        _globalIdLocalEntry = CreateEntry("Código IPMA da localidade (Coimbra: 1060300)", 12);
        _globalIdLocalEntry.Text = "1060300";
        _globalIdLocalEntry.Keyboard = Keyboard.Numeric;
        _errorLabel = new Label
        {
            IsVisible = false,
            FontSize = 13,
            TextColor = ViewTheme.Error
        };

        var cancelButton = CreateButton("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancelButton.Clicked += (_, _) => Cancel();
        var createButton = CreateButton("Criar evento", Colors.White, ViewTheme.Accent);
        createButton.Clicked += (_, _) => Save();

        var buttons = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children = { cancelButton, createButton }
        };
        Grid.SetColumn(createButton, 1);

        Content = new Border
        {
            WidthRequest = 360,
            MaximumWidthRequest = 520,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 5,
                        Children =
                        {
                            new Label
                            {
                                Text = "Novo evento",
                                FontSize = 22,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = ViewTheme.Text
                            },
                            new Label
                            {
                                Text = $"Evento para {AppSession.FormatShortDate(AppSession.SelectedDate)}.",
                                FontSize = 14,
                                TextColor = ViewTheme.Muted
                            }
                        }
                    },
                    _titleEntry,
                    _locationEntry,
                    _globalIdLocalEntry,
                    _errorLabel,
                    buttons
                }
            }
        };
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
        var title = _titleEntry.Text?.Trim();
        var location = _locationEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(title)
            || string.IsNullOrWhiteSpace(location)
            || !int.TryParse(_globalIdLocalEntry.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var globalIdLocal)
            || globalIdLocal <= 0)
        {
            _errorLabel.Text = "Introduza o nome, a localização e um código IPMA válido.";
            _errorLabel.IsVisible = true;
            return;
        }

        _completion.TrySetResult(new EventDraft(
            title,
            location,
            globalIdLocal));
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));
}

public sealed record EventDraft(string Title, string Description, int GlobalIdLocal);

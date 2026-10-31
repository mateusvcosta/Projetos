using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class AddInventoryTablePopup : ContentView
{
    private readonly TaskCompletionSource<string?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Entry _nameEntry;
    private readonly Label _errorLabel;

    public Task<string?> Completion => _completion.Task;

    public AddInventoryTablePopup()
    {
        BackgroundColor = Colors.Transparent;
        Padding = 0;

        _nameEntry = new Entry
        {
            Placeholder = "Nome da tabela",
            MaxLength = 80,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        _errorLabel = new Label
        {
            IsVisible = false,
            FontSize = 13,
            TextColor = ViewTheme.Error
        };

        var cancelButton = CreateButton("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancelButton.Clicked += (_, _) => Cancel();
        var createButton = CreateButton("Criar", Colors.White, ViewTheme.Accent);
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
            WidthRequest = 320,
            MaximumWidthRequest = 460,
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
                                Text = "Nova tabela",
                                FontSize = 22,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = ViewTheme.Text
                            },
                            new Label
                            {
                                Text = "Adicione uma categoria ao inventário.",
                                FontSize = 14,
                                TextColor = ViewTheme.Muted
                            }
                        }
                    },
                    _nameEntry,
                    _errorLabel,
                    buttons
                }
            }
        };
    }

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
        if (string.IsNullOrWhiteSpace(name))
        {
            _errorLabel.Text = "Introduza o nome da tabela.";
            _errorLabel.IsVisible = true;
            return;
        }

        _completion.TrySetResult(name);
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));
}

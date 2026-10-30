using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class AddDocumentPopup : ContentView
{
    private readonly TaskCompletionSource<DocumentDraft?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Entry _typeEntry;
    private readonly Entry _typeCodeEntry;
    private readonly Entry _pathEntry;
    private readonly Editor _notesEditor;
    private readonly Label _errorLabel;

    public Task<DocumentDraft?> Completion => _completion.Task;

    public AddDocumentPopup()
    {
        BackgroundColor = Colors.Transparent;
        Padding = 0;
        _typeEntry = CreateEntry("Tipo de documento", 100);
        _typeCodeEntry = CreateEntry("ID do tipo (ex.: TIP-0001)", 50);
        _pathEntry = CreateEntry("Caminho ou URL do ficheiro, opcional", 300);
        _notesEditor = new Editor
        {
            Placeholder = "Observações, opcional",
            MaxLength = 500,
            HeightRequest = 90,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        _errorLabel = new Label { IsVisible = false, FontSize = 13, TextColor = ViewTheme.Error };

        var cancelButton = CreateButton("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancelButton.Clicked += (_, _) => Cancel();
        var saveButton = CreateButton("Guardar", Colors.White, ViewTheme.Accent);
        saveButton.Clicked += (_, _) => Save();
        var buttons = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children = { cancelButton, saveButton }
        };
        Grid.SetColumn(saveButton, 1);

        Content = new Border
        {
            WidthRequest = 420,
            MaximumWidthRequest = 560,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new ScrollView
            {
                MaximumHeightRequest = 650,
                Content = new VerticalStackLayout
                {
                    Spacing = 14,
                    Children =
                    {
                        new Label
                        {
                            Text = "Novo documento",
                            FontSize = 22,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = ViewTheme.Text
                        },
                        new Label
                        {
                            Text = "Adicione os dados do documento.",
                            FontSize = 14,
                            TextColor = ViewTheme.Muted
                        },
                        _typeEntry, _typeCodeEntry, _pathEntry, _notesEditor, _errorLabel, buttons
                    }
                }
            }
        };
    }

    private static Entry CreateEntry(string placeholder, int maxLength) => new()
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
            Text = text, TextColor = textColor, BackgroundColor = backgroundColor, CornerRadius = 10
        };
        SemanticProperties.SetDescription(button, text);
        return button;
    }

    private void Save()
    {
        var type = _typeEntry.Text?.Trim();
        var typeCode = _typeCodeEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(typeCode))
        {
            _errorLabel.Text = "Introduza o tipo e o ID do tipo do documento.";
            _errorLabel.IsVisible = true;
            return;
        }

        _completion.TrySetResult(new DocumentDraft(
            type, typeCode, _pathEntry.Text?.Trim() ?? "", _notesEditor.Text?.Trim() ?? ""));
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));
}

public sealed record DocumentDraft(string Type, string TypeCode, string FilePath, string Notes);

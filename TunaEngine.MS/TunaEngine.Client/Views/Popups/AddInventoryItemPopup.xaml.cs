using Microsoft.Maui.Controls.Shapes;
using System.Globalization;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class AddInventoryItemPopup : ContentView
{
    private readonly TaskCompletionSource<InventoryItemDraft?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;
    private readonly Entry _nameEntry;
    private readonly Entry _brandEntry;
    private readonly Entry _unitCostEntry;
    private readonly Entry _conditionEntry;
    private readonly Entry _functionEntry;
    private readonly Entry _notesEntry;
    private readonly Label _errorLabel;
    private readonly StateButton _saveButton;

    public Task<InventoryItemDraft?> Completion => _completion.Task;

    public AddInventoryItemPopup(
        string? name = null,
        string? brand = null,
        float? unitCost = null,
        string? condition = null,
        string? function = null,
        string? notes = null)
    {
        BackgroundColor = Colors.Transparent;
        Padding = 0;

        _titleLabel = new Label
        {
            Text = "Novo item",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = ViewTheme.Text
        };
        _descriptionLabel = new Label
        {
            Text = "Adicione um item ao inventário.",
            FontSize = 14,
            TextColor = ViewTheme.Muted
        };
        _nameEntry = CreateEntry("Nome do item", maxLength: 80);
        _brandEntry = CreateEntry("Marca (obrigatória)", maxLength: 80);
        _unitCostEntry = CreateEntry("Custo unitário (€), opcional", maxLength: 12);
        _conditionEntry = CreateEntry("Estado, opcional", maxLength: 100);
        _functionEntry = CreateEntry("Função, opcional", maxLength: 100);
        _notesEntry = CreateEntry("Observações, opcional", maxLength: 250);
        _errorLabel = new Label
        {
            IsVisible = false,
            FontSize = 13,
            TextColor = ViewTheme.Error
        };

        var cancelButton = CreateButton("Cancelar", "Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancelButton.Clicked += (_, _) => Cancel();
        _saveButton = CreateButton("Guardar", "Guardar", Colors.White, ViewTheme.Accent);
        _saveButton.Clicked += (_, _) => Save();

        var fields = new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                _nameEntry, _brandEntry, _unitCostEntry,
                _conditionEntry, _functionEntry, _notesEntry, _errorLabel
            }
        };
        var buttons = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children = { cancelButton, _saveButton }
        };
        Grid.SetColumn(_saveButton, 1);

        var content = new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                new VerticalStackLayout
                {
                    Spacing = 5,
                    Children = { _titleLabel, _descriptionLabel }
                },
                fields,
                buttons
            }
        };

        Content = new Border
        {
            WidthRequest = 320,
            MaximumWidthRequest = 460,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = content
        };

        if (name is null)
        {
            return;
        }

        _titleLabel.Text = "Editar item";
        _descriptionLabel.Text = "Atualize os dados do item.";
        _nameEntry.Text = name;
        _brandEntry.Text = brand;
        _unitCostEntry.Text = unitCost?.ToString("0.00", CultureInfo.GetCultureInfo("pt-PT"));
        _conditionEntry.Text = condition;
        _functionEntry.Text = function;
        _notesEntry.Text = notes;
        _saveButton.Text = "Guardar alterações";
    }

    private static Entry CreateEntry(string placeholder, int maxLength, Keyboard? keyboard = null) =>
        new()
        {
            Placeholder = placeholder,
            MaxLength = maxLength,
            Keyboard = keyboard ?? Keyboard.Default,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark
        };

    private static StateButton CreateButton(string text, string automationId, Color textColor, Color backgroundColor)
    {
        var button = new StateButton
        {
            Text = text,
            AutomationId = automationId,
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
        var brand = _brandEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(brand)
            || !TryParseUnitCost(_unitCostEntry.Text, out var unitCost))
        {
            _errorLabel.Text = "Introduza o nome, a marca e um custo válido (por exemplo, 12,50).";
            _errorLabel.IsVisible = true;
            return;
        }

        _completion.TrySetResult(new InventoryItemDraft(
            name,
            brand,
            unitCost,
            _conditionEntry.Text?.Trim(),
            _functionEntry.Text?.Trim(),
            _notesEntry.Text?.Trim()));
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));

    private static bool TryParseUnitCost(string? value, out float? unitCost)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            unitCost = null;
            return true;
        }

        var normalizedValue = value?.Trim().Replace(',', '.');
        if (float.TryParse(
            normalizedValue,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out var parsedCost) && parsedCost >= 0)
        {
            unitCost = parsedCost;
            return true;
        }

        unitCost = null;
        return false;
    }

}

public sealed record InventoryItemDraft(
    string Name,
    string? Brand,
    float? UnitCost,
    string? Condition,
    string? Function,
    string? Notes);

using CommunityToolkit.Maui.Views;
using System.Globalization;

namespace TunaEngine.Views.Popups;

public partial class AddInventoryItemPopup : Popup
{
    public InventoryItemDraft? Item { get; private set; }

    public AddInventoryItemPopup(string? name = null, int quantity = 1, decimal? unitCost = null)
    {
        InitializeComponent();

        if (name is null)
        {
            return;
        }

        TitleLabel.Text = "Editar item";
        DescriptionLabel.Text = "Atualize os dados do item.";
        NameEntry.Text = name;
        QuantityEntry.Text = quantity.ToString(CultureInfo.CurrentCulture);
        UnitCostEntry.Text = unitCost?.ToString("0.00", CultureInfo.GetCultureInfo("pt-PT"));
        SaveButton.Text = "Guardar alterações";
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name)
            || !int.TryParse(QuantityEntry.Text, out var quantity)
            || quantity < 1
            || !TryParseUnitCost(UnitCostEntry.Text, out var unitCost)
            || unitCost < 0)
        {
            ErrorLabel.Text = "Introduza o nome, uma quantidade válida e um custo unitário não negativo.";
            ErrorLabel.IsVisible = true;
            return;
        }

        Item = new InventoryItemDraft(name, quantity, unitCost);
        await CloseAsync();
    }

    private static bool TryParseUnitCost(string? value, out decimal unitCost)
    {
        var normalizedValue = value?.Trim().Replace(',', '.');
        return decimal.TryParse(
            normalizedValue,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out unitCost);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync();
    }
}

public sealed record InventoryItemDraft(string Name, int Quantity, decimal UnitCost);
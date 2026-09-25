using CommunityToolkit.Maui.Views;

namespace DesktopApp.Maui.Views.Popups;

public partial class AddInventoryItemPopup : Popup
{
    public InventoryItemDraft? Item { get; private set; }

    public AddInventoryItemPopup()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name)
            || !int.TryParse(QuantityEntry.Text, out var quantity)
            || quantity < 1)
        {
            ErrorLabel.Text = "Introduza o nome e uma quantidade inteira maior que zero.";
            ErrorLabel.IsVisible = true;
            return;
        }

        Item = new InventoryItemDraft(name, quantity);
        await CloseAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync();
    }
}

public sealed record InventoryItemDraft(string Name, int Quantity);
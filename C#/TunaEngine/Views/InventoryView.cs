using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using TunaEngine.Views.Popups;
using System.Globalization;

namespace TunaEngine.Views;

public sealed class InventoryView : ContentView
{
    private readonly List<InventoryItem> items =
    [
        new("Cabo XLR", 4),
        new("Microfone", 2),
        new("Suporte", 3)
    ];

    private readonly VerticalStackLayout itemRows = new() { Spacing = 0 };
    private readonly Label totalCostLabel = new()
    {
        FontSize = 22,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#F3F5F4"),
        HorizontalTextAlignment = TextAlignment.End,
        VerticalTextAlignment = TextAlignment.Center
    };

    public InventoryView(bool isWideLayout)
    {
        var content = new VerticalStackLayout { Spacing = 18 };
        var titleLabel = new Label
        {
            Text = "Inventário",
            FontSize = isWideLayout ? 34 : 29,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F3F5F4")
        };
        SemanticProperties.SetHeadingLevel(titleLabel, SemanticHeadingLevel.Level1);

        var addButton = new Button
        {
            Text = "Novo item",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#D94F65"),
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 6,
            Padding = new Thickness(14, 10)
        };

        var addButtonTable = new Button
        {
            Text = "Nova Tabela",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#D94F65"),
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 6,
            Padding = new Thickness(14, 10)
        };

        var titleRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star)
            },
            VerticalOptions = LayoutOptions.Center
        };
        titleRow.Add(titleLabel, 0, 0);

        var totalCostSummary = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            Padding = new Thickness(14, 12),
            BackgroundColor = Color.FromArgb("#1A262E")
        };
        totalCostSummary.Add(new Label
        {
            Text = "CUSTO TOTAL DO INVENTÁRIO",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#91A0A5"),
            VerticalTextAlignment = TextAlignment.Center
        }, 0, 0);
        totalCostSummary.Add(totalCostLabel, 1, 0);
        var totalCostBorder = new Border
        {
            BackgroundColor = Color.FromArgb("#1A262E"),
            Stroke = Color.FromArgb("#2D3A42"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = totalCostSummary
        };

        addButton.Clicked += async (_, _) =>
        {
            var popup = new AddInventoryItemPopup();
            if (await ShowItemPopupAsync(popup) is { } item)
            {
                items.Add(new InventoryItem(item.Name, item.Quantity, item.UnitCost));
                RefreshItemRows();
            }
        };

        content.Children.Add(titleRow);
        content.Children.Add(totalCostBorder);
        content.Children.Add(CreateTable(addButton));
        content.Children.Add(addButtonTable);
        Content = content;
        RefreshItemRows();
    }

    private Border CreateTable(Button addItemButton)
    {
        var table = new VerticalStackLayout { Spacing = 0 };
        var tableTitle = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            Padding = new Thickness(12, 8),
            BackgroundColor = Color.FromArgb("#1A262E")
        };
        tableTitle.Add(CreateTableLabel("ITENS", true), 0, 0);
        tableTitle.Add(addItemButton, 1, 0);
        table.Children.Add(tableTitle);
        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(68)),
                new ColumnDefinition(new GridLength(112)),
                new ColumnDefinition(new GridLength(126))
            },
            ColumnSpacing = 6,
            Padding = new Thickness(8, 12),
            BackgroundColor = Color.FromArgb("#202D35")
        };
        header.Add(CreateTableLabel("ITEM", true), 0, 0);
        header.Add(CreateTableLabel("QTD.", true, TextAlignment.End), 1, 0);
        header.Add(CreateTableLabel("CUSTO/UN.", true, TextAlignment.End), 2, 0);
        header.Add(CreateTableLabel("", true), 3, 0);
        table.Children.Add(header);
        table.Children.Add(new BoxView
        {
            HeightRequest = 1,
            BackgroundColor = Color.FromArgb("#2D3A42")
        });
        table.Children.Add(itemRows);

        return new Border
        {
            BackgroundColor = Color.FromArgb("#1A262E"),
            Stroke = Color.FromArgb("#2D3A42"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = table
        };
    }

    private void RefreshItemRows()
    {
        itemRows.Children.Clear();
        if (items.Count == 0)
        {
            itemRows.Children.Add(new Label
            {
                Text = "Nenhum item no inventário.",
                FontSize = 14,
                TextColor = Color.FromArgb("#91A0A5"),
                Margin = new Thickness(14, 16)
            });
            RefreshTotalCost();
            return;
        }

        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(68)),
                    new ColumnDefinition(new GridLength(112)),
                    new ColumnDefinition(new GridLength(126))
                },
                ColumnSpacing = 6,
                Padding = new Thickness(8, 8)
            };
            row.Add(CreateTableLabel(item.Name), 0, 0);
            row.Add(CreateTableLabel(item.Quantity.ToString(), false, TextAlignment.End), 1, 0);
            var unitCostLabel = CreateTableLabel(
                item.UnitCost?.ToString("C", CultureInfo.GetCultureInfo("pt-PT")) ?? "—",
                false,
                TextAlignment.End);
            row.Add(unitCostLabel, 2, 0);
            var actions = new HorizontalStackLayout { Spacing = 0 };
            var editButton = new Button
            {
                Text = "Editar",
                FontSize = 11,
                Padding = new Thickness(4, 8),
                MinimumWidthRequest = 54,
                MinimumHeightRequest = 44,
                CornerRadius = 4,
                TextColor = Color.FromArgb("#F3F5F4"),
                BackgroundColor = Colors.Transparent,
                AutomationId = $"EditInventoryItem{index}"
            };
            editButton.Clicked += async (_, _) =>
            {
                var popup = new AddInventoryItemPopup(item.Name, item.Quantity, item.UnitCost);
                if (await ShowItemPopupAsync(popup) is { } updatedItem)
                {
                    items[index] = new InventoryItem(updatedItem.Name, updatedItem.Quantity, updatedItem.UnitCost);
                    RefreshItemRows();
                }
            };
            actions.Children.Add(editButton);

            var removeButton = new Button
            {
                Text = "Remover",
                FontSize = 11,
                Padding = new Thickness(4, 8),
                MinimumWidthRequest = 44,
                MinimumHeightRequest = 44,
                CornerRadius = 4,
                TextColor = Color.FromArgb("#E88E9A"),
                BackgroundColor = Colors.Transparent,
                AutomationId = $"RemoveInventoryItem{index}"
            };
            removeButton.Clicked += (_, _) =>
            {
                items.Remove(item);
                RefreshItemRows();
            };
            actions.Children.Add(removeButton);
            row.Add(actions, 3, 0);
            itemRows.Children.Add(row);
            itemRows.Children.Add(new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#2D3A42")
            });
        }

        RefreshTotalCost();
    }

    private static async Task<InventoryItemDraft?> ShowItemPopupAsync(AddInventoryItemPopup popup)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return null;
        }

        await shell.ShowPopupAsync(popup, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = false,
            PageOverlayColor = Color.FromArgb("#B0000000"),
            Shape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 8,
                Fill = Color.FromArgb("#172129"),
                Stroke = Color.FromArgb("#172129"),
                StrokeThickness = 0
            },
            Shadow = null
        });

        return popup.Item;
    }

    private void RefreshTotalCost()
    {
        if (items.Any(item => item.UnitCost is null))
        {
            totalCostLabel.Text = "Custos em falta";
            totalCostLabel.FontSize = 14;
            return;
        }

        var total = items.Sum(item => item.Quantity * item.UnitCost!.Value);
        totalCostLabel.Text = total.ToString("C", CultureInfo.GetCultureInfo("pt-PT"));
        totalCostLabel.FontSize = 22;
    }

    private static Label CreateTableLabel(string text, bool isHeader = false, TextAlignment alignment = TextAlignment.Start)
    {
        return new Label
        {
            Text = text,
            FontSize = isHeader ? 11 : 14,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            TextColor = isHeader ? Color.FromArgb("#91A0A5") : Color.FromArgb("#F3F5F4"),
            HorizontalTextAlignment = alignment,
            VerticalTextAlignment = TextAlignment.Center
        };
    }

    private sealed record InventoryItem(string Name, int Quantity, decimal? UnitCost = null);
}
using System.Globalization;

namespace TunaEngine.Views;

public sealed class InventoryView : ContentView
{
    public InventoryView()
    {
        Render();
    }

    private void Render()
    {
        var content = new VerticalStackLayout { Spacing = 0 };
        var title = ViewTheme.TextLabel("Inventário", 30, true);
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        title.HeightRequest = 36;
        title.Margin = new Thickness(0, 0, 0, 12);
        content.Children.Add(title);

        var items = AppSession.Tables.SelectMany(table => table.Items).ToList();
        var total = items.Any(item => !item.UnitCost.HasValue)
            ? "Custos em falta"
            : AppSession.Money(items.Sum(item => item.Quantity * item.UnitCost.GetValueOrDefault()));
        var summaryContent = new VerticalStackLayout
        {
            Spacing = 6,
            Padding = new Thickness(20, 16),
            Children =
            {
                ViewTheme.TextLabel("CUSTO TOTAL DO INVENTÁRIO", 11, true, ViewTheme.Muted),
                ViewTheme.TextLabel(total, 22, true)
            }
        };
        var summary = ViewTheme.Card(summaryContent, 0);
        summary.WidthRequest = 520;
        summary.HeightRequest = 72;
        summary.HorizontalOptions = LayoutOptions.Start;

        var newItemButton = ViewTheme.Action("+ Novo item", async () =>
        {
            if (AppSession.Tables.Count > 0)
                await EditItem(AppSession.Tables[0], null);
        });
        newItemButton.WidthRequest = 220;
        newItemButton.HeightRequest = 48;
        newItemButton.MinimumHeightRequest = 48;
        newItemButton.FontSize = 15;
        newItemButton.Padding = 0;

        var summaryRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 0,
            HeightRequest = 72,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 24)
        };
        summaryRow.Add(summary, 0, 0);
        summaryRow.Add(newItemButton, 1, 0);
        content.Children.Add(summaryRow);

        var tables = new VerticalStackLayout
        {
            Spacing = 16,
            HorizontalOptions = LayoutOptions.Fill
        };
        foreach (var table in AppSession.Tables)
            tables.Children.Add(BuildInventoryTable(table));
        content.Children.Add(tables);

        var newTableButton = ViewTheme.Action("+ Nova Tabela", AddTable);
        newTableButton.WidthRequest = 260;
        newTableButton.HeightRequest = 48;
        newTableButton.MinimumHeightRequest = 48;
        newTableButton.FontSize = 15;
        newTableButton.Padding = 0;
        newTableButton.Margin = new Thickness(0, 24, 0, 0);
        newTableButton.HorizontalOptions = LayoutOptions.Start;
        content.Children.Add(newTableButton);
        Content = content;
    }

    private View BuildInventoryTable(StockTable table)
    {
        var columns = new[]
        {
            new GridLength(480, GridUnitType.Star),
            new GridLength(100, GridUnitType.Star),
            new GridLength(140, GridUnitType.Star),
            new GridLength(140, GridUnitType.Star),
            new GridLength(120, GridUnitType.Star),
            new GridLength(120, GridUnitType.Star)
        };

        Grid CreateRow(params View[] cells)
        {
            var grid = new Grid
            {
                ColumnSpacing = 0,
                Padding = new Thickness(20, 0),
                HeightRequest = 64
            };

            foreach (var width in columns)
                grid.ColumnDefinitions.Add(new ColumnDefinition(width));

            for (var index = 0; index < cells.Length; index++)
                grid.Add(cells[index], index, 0);

            return grid;
        }

        var panel = new VerticalStackLayout { Spacing = 0 };
        var sectionHeader = new Grid
        {
            Padding = new Thickness(20, 0),
            HeightRequest = 40,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        var panelTitle = ViewTheme.TextLabel(
            table.Name.ToUpper(AppSession.Portuguese), 11, true, ViewTheme.Muted);
        sectionHeader.Add(panelTitle, 0, 0);
        sectionHeader.Add(new BoxView
        {
            HeightRequest = 1,
            Color = ViewTheme.Stroke,
            VerticalOptions = LayoutOptions.End
        }, 0, 0);
        panel.Children.Add(sectionHeader);

        var headings = CreateRow(
            ViewTheme.TextLabel("Item", 12, true, ViewTheme.Muted),
            AlignedLabel("Qtd.", TextAlignment.End),
            AlignedLabel("Custo/Un.", TextAlignment.End),
            AlignedLabel("Total", TextAlignment.End),
            CenteredLabel("Editar", ViewTheme.Muted, 12, true),
            CenteredLabel("Remover", ViewTheme.Muted, 12, true));
        headings.HeightRequest = 44;
        headings.BackgroundColor = ViewTheme.SurfaceDark;
        var headingsDivider = new BoxView
        {
            HeightRequest = 1,
            Color = ViewTheme.Stroke,
            VerticalOptions = LayoutOptions.End
        };
        Grid.SetColumnSpan(headingsDivider, headings.ColumnDefinitions.Count);
        headings.Add(headingsDivider, 0, 0);
        panel.Children.Add(headings);

        for (var index = 0; index < table.Items.Count; index++)
        {
            var item = table.Items[index];
            var capturedItem = item;
            var icon = ItemIcon(item.Name);

            var itemCell = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(32)),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 12
            };
            var itemIcon = new Border
            {
                WidthRequest = 32,
                HeightRequest = 32,
                BackgroundColor = Color.FromArgb("#333355"),
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                Content = new Image
                {
                    Source = icon,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    Aspect = Aspect.AspectFit
                }
            };
            itemCell.Add(itemIcon, 0, 0);
            itemCell.Add(ViewTheme.TextLabel(item.Name, 14), 1, 0);

            var editButton = TableAction("pencil.svg", "Editar", ViewTheme.Muted,
                () => EditItem(table, capturedItem));
            var removeButton = TableAction("trash.svg", "Remover", ViewTheme.Accent,
                () => RemoveItem(table, capturedItem));
            var row = CreateRow(
                itemCell,
                AlignedLabel(item.Quantity.ToString(), TextAlignment.End, Color.FromArgb("#CCCCDD"), 14),
                AlignedLabel(AppSession.Money(item.UnitCost), TextAlignment.End, ViewTheme.Muted, 14),
                AlignedLabel(AppSession.Money(
                    item.UnitCost.HasValue ? item.UnitCost.Value * item.Quantity : null),
                    TextAlignment.End, ViewTheme.Muted, 14),
                editButton,
                removeButton);
            row.HeightRequest = 64;
            var rowDivider = new BoxView
            {
                HeightRequest = 1,
                Color = ViewTheme.Stroke,
                VerticalOptions = LayoutOptions.End
            };
            Grid.SetColumnSpan(rowDivider, row.ColumnDefinitions.Count);
            row.Add(rowDivider, 0, 0);
            panel.Children.Add(row);
        }

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            HorizontalOptions = LayoutOptions.Fill,
            Content = panel
        };
    }

    private static Label CenteredLabel(
        string text,
        Color? color = null,
        double size = 10,
        bool bold = false)
    {
        var label = ViewTheme.TextLabel(text, size, bold, color ?? ViewTheme.Muted);
        label.HorizontalTextAlignment = TextAlignment.Center;
        return label;
    }

    private static Label AlignedLabel(
        string text,
        TextAlignment alignment,
        Color? color = null,
        double size = 12,
        bool bold = true)
    {
        var label = ViewTheme.TextLabel(text, size, bold, color ?? ViewTheme.Muted);
        label.HorizontalTextAlignment = alignment;
        return label;
    }

    private static string ItemIcon(string itemName)
    {
        if (itemName.Contains("mic", StringComparison.OrdinalIgnoreCase))
            return "microphone.svg";
        if (itemName.Contains("suporte", StringComparison.OrdinalIgnoreCase) ||
            itemName.Contains("stand", StringComparison.OrdinalIgnoreCase))
            return "stand.svg";
        return "music.svg";
    }

    private static View TableAction(string icon, string text, Color color, Func<Task> callback)
    {
        var visual = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(16)),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 6,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        visual.Add(new Image
        {
            Source = icon,
            WidthRequest = 16,
            HeightRequest = 16,
            Aspect = Aspect.AspectFit
        }, 0, 0);
        visual.Add(ViewTheme.TextLabel(text, 13, true, color), 1, 0);

        var button = new Button
        {
            Text = "",
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CornerRadius = 0,
            MinimumHeightRequest = 64,
            MinimumWidthRequest = 0,
            Opacity = 0
        };
        SemanticProperties.SetDescription(button, text);
        button.Clicked += async (_, _) => await callback();

        var action = new Grid { HeightRequest = 64 };
        action.Add(visual);
        action.Add(button);
        return action;
    }

    private async Task AddTable()
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var name = await shell.DisplayPromptAsync("Nova tabela", "Nome da tabela:", "Criar", "Cancelar");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        AppSession.Tables.Add(new StockTable { Name = name.Trim() });
        Render();
    }

    private async Task EditItem(StockTable table, StockItem? item)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var name = await shell.DisplayPromptAsync(
            item is null ? "Novo item" : "Editar item",
            "Nome:",
            "Continuar",
            "Cancelar",
            initialValue: item?.Name ?? "");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var quantityText = await shell.DisplayPromptAsync(
            "Quantidade",
            "Introduza uma quantidade inteira:",
            "Continuar",
            "Cancelar",
            keyboard: Keyboard.Numeric,
            initialValue: item?.Quantity.ToString() ?? "1");
        if (quantityText is null)
        {
            return;
        }

        if (!int.TryParse(quantityText, out var quantity) || quantity < 0)
        {
            await shell.DisplayAlertAsync(
                "Quantidade inválida",
                "Utilize um número inteiro igual ou superior a zero.",
                "OK");
            return;
        }

        var costText = await shell.DisplayPromptAsync(
            "Custo por unidade",
            "Deixe vazio se o custo for desconhecido.",
            "Guardar",
            "Cancelar",
            keyboard: Keyboard.Numeric,
            initialValue: item?.UnitCost?.ToString(AppSession.Portuguese) ?? "");
        if (costText is null)
        {
            return;
        }

        decimal? cost = null;
        if (!string.IsNullOrWhiteSpace(costText))
        {
            if (!decimal.TryParse(
                    costText,
                    NumberStyles.Number,
                    AppSession.Portuguese,
                    out var parsedCost) ||
                parsedCost < 0)
            {
                await shell.DisplayAlertAsync(
                    "Custo inválido",
                    "Utilize um valor positivo, por exemplo 12,50.",
                    "OK");
                return;
            }

            cost = parsedCost;
        }

        item ??= new StockItem();
        item.Name = name.Trim();
        item.Quantity = quantity;
        item.UnitCost = cost;
        if (!table.Items.Contains(item))
        {
            table.Items.Add(item);
        }

        Render();
    }

    private async Task RemoveItem(StockTable table, StockItem item)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var confirmed = await shell.DisplayAlertAsync(
            "Remover item",
            $"Remover “{item.Name}”?",
            "Remover",
            "Cancelar");
        if (!confirmed)
        {
            return;
        }

        table.Items.Remove(item);
        Render();
    }
}

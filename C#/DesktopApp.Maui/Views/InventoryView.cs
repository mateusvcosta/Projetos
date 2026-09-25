namespace DesktopApp.Maui.Views;

public sealed class InventoryView : ContentView
{
    private readonly List<InventoryItem> items =
    [
        new("Cabo XLR", 4),
        new("Microfone", 2),
        new("Suporte", 3)
    ];

    private readonly VerticalStackLayout itemRows = new() { Spacing = 0 };

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

        var titleRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            VerticalOptions = LayoutOptions.Center
        };
        titleRow.Add(titleLabel, 0, 0);
        titleRow.Add(addButton, 1, 0);
        content.Children.Add(titleRow);
        content.Children.Add(new Label
        {
            Text = "Itens guardados pela equipa.",
            FontSize = 15,
            TextColor = Color.FromArgb("#AAB7BA")
        });

        var nameEntry = CreateEntry("Nome do item");
        var quantityEntry = CreateEntry("Quantidade");
        quantityEntry.Keyboard = Keyboard.Numeric;
        quantityEntry.Text = "1";
        var formMessage = new Label
        {
            TextColor = Color.FromArgb("#E88E9A"),
            FontSize = 13,
            IsVisible = false
        };

        var saveButton = new Button
        {
            Text = "Guardar",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#D94F65"),
            CornerRadius = 6
        };
        var cancelButton = new Button
        {
            Text = "Cancelar",
            TextColor = Color.FromArgb("#F3F5F4"),
            BackgroundColor = Color.FromArgb("#2D3A42"),
            CornerRadius = 6
        };
        var formActions = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.End,
            Children = { cancelButton, saveButton }
        };
        var formContent = new VerticalStackLayout
        {
            Spacing = 10,
            Children = { nameEntry, quantityEntry, formMessage, formActions }
        };
        var addForm = new Border
        {
            IsVisible = false,
            BackgroundColor = Color.FromArgb("#172129"),
            Stroke = Color.FromArgb("#2D3A42"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Padding = 16,
            Content = formContent
        };

        addButton.Clicked += (_, _) =>
        {
            addForm.IsVisible = true;
            nameEntry.Focus();
        };
        cancelButton.Clicked += (_, _) =>
        {
            addForm.IsVisible = false;
            formMessage.IsVisible = false;
            nameEntry.Text = string.Empty;
            quantityEntry.Text = "1";
        };
        saveButton.Clicked += (_, _) =>
        {
            var name = nameEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name)
                || !int.TryParse(quantityEntry.Text, out var quantity)
                || quantity < 1)
            {
                formMessage.Text = "Introduza o nome e uma quantidade válida.";
                formMessage.IsVisible = true;
                return;
            }

            items.Add(new InventoryItem(name, quantity));
            RefreshItemRows();
            addForm.IsVisible = false;
            formMessage.IsVisible = false;
            nameEntry.Text = string.Empty;
            quantityEntry.Text = "1";
        };

        content.Children.Add(addForm);
        content.Children.Add(new Label
        {
            Text = "ITENS",
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            CharacterSpacing = 1.5,
            TextColor = Color.FromArgb("#91A0A5"),
            Margin = new Thickness(0, 12, 0, 0)
        });
        content.Children.Add(CreateTable());
        Content = content;
        RefreshItemRows();
    }

    private static Entry CreateEntry(string placeholder)
    {
        return new Entry
        {
            Placeholder = placeholder,
            TextColor = Color.FromArgb("#F3F5F4"),
            PlaceholderColor = Color.FromArgb("#91A0A5"),
            BackgroundColor = Color.FromArgb("#1A262E")
        };
    }

    private Border CreateTable()
    {
        var table = new VerticalStackLayout { Spacing = 0 };
        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(84)),
                new ColumnDefinition(new GridLength(78))
            },
            ColumnSpacing = 6,
            Padding = new Thickness(8, 12),
            BackgroundColor = Color.FromArgb("#202D35")
        };
        header.Add(CreateTableLabel("ITEM", true), 0, 0);
        header.Add(CreateTableLabel("QUANTIDADE", true, TextAlignment.End), 1, 0);
        header.Add(CreateTableLabel("", true), 2, 0);
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
            return;
        }

        foreach (var item in items)
        {
            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(84)),
                    new ColumnDefinition(new GridLength(78))
                },
                ColumnSpacing = 6,
                Padding = new Thickness(8, 8)
            };
            row.Add(CreateTableLabel(item.Name), 0, 0);
            row.Add(CreateTableLabel(item.Quantity.ToString(), false, TextAlignment.End), 1, 0);
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
                AutomationId = $"RemoveInventoryItem{items.IndexOf(item)}"
            };
            removeButton.Clicked += (_, _) =>
            {
                items.Remove(item);
                RefreshItemRows();
            };
            row.Add(removeButton, 2, 0);
            itemRows.Children.Add(row);
            itemRows.Children.Add(new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#2D3A42")
            });
        }
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

    private sealed record InventoryItem(string Name, int Quantity);
}
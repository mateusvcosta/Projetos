using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;
using TunaEngine.Views.Popups;

namespace TunaEngine.Views;

public sealed class InventoryView : ContentView
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Label _status = ViewTheme.TextLabel("A carregar inventário...", 13, color: ViewTheme.Muted);
    private readonly Button _retryButton;
    private readonly Label _totalLabel = ViewTheme.TextLabel("", 22, true);
    private readonly VerticalStackLayout _tables = new()
    {
        Spacing = 16,
        HorizontalOptions = LayoutOptions.Fill
    };
    private bool _isInitialized;

    public InventoryView(TunaEngineApiClient apiClient)
    {
        _apiClient = apiClient;
        _retryButton = ViewTheme.Action("Tentar novamente", LoadInventoryAsync, primary: false);
        _retryButton.IsVisible = false;
        Render();
        _ = LoadInventoryAsync();
    }

    private void Render()
    {
        if (_isInitialized)
        {
            RefreshItems();
            return;
        }

        _isInitialized = true;
        var content = new VerticalStackLayout { Spacing = 0 };
        var title = ViewTheme.TextLabel("Inventário", 30, true);
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        title.HeightRequest = 36;
        title.Margin = new Thickness(0, 0, 0, 12);
        content.Children.Add(title);
        content.Children.Add(_status);
        content.Children.Add(_retryButton);

        var summaryContent = new VerticalStackLayout
        {
            Spacing = 6,
            Padding = new Thickness(20, 16),
            Children =
            {
                ViewTheme.TextLabel("CUSTO TOTAL DO INVENTÁRIO", 11, true, ViewTheme.Muted),
                _totalLabel
            }
        };
        var summary = ViewTheme.Card(summaryContent, 0);
        summary.WidthRequest = 520;
        summary.HeightRequest = 72;
        summary.HorizontalOptions = LayoutOptions.Start;

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
        content.Children.Add(summaryRow);

        content.Children.Add(_tables);

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
        RefreshItems();
    }

    private void RefreshItems()
    {
        var items = AppSession.Tables.SelectMany(table => table.Items).ToList();
        _totalLabel.Text = items.Any(item => !item.UnitCost.HasValue)
            ? "Total indisponível"
            : AppSession.Money(items.Sum(item => item.UnitCost.GetValueOrDefault()));

        AppSession.Tables.Sort(CompareTableNames);
        _tables.Children.Clear();
        foreach (var table in AppSession.Tables)
        {
            _tables.Children.Add(BuildInventoryTable(table));
        }
    }

    private async Task LoadInventoryAsync()
    {
        _status.Text = "A carregar inventário...";
        _status.TextColor = ViewTheme.Muted;
        _retryButton.IsVisible = false;

        try
        {
            var items = await _apiClient.GetAsync<List<ApiInventoryItem>>("api/inventory");
            AppSession.Tables.Clear();
            foreach (var group in items
                         .GroupBy(item =>
                             string.IsNullOrWhiteSpace(item.InstrumentType)
                                 ? "Itens"
                                 : item.InstrumentType.Trim())
                         .OrderBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase))
            {
                var table = new StockTable { Name = group.Key };
                table.Items.AddRange(group.Select(item => new StockItem
                {
                    Id = item.Id,
                    Name = item.Designation,
                    Brand = item.Brand,
                    UnitCost = item.Price,
                    Condition = item.Condition,
                    Function = item.Function,
                    Notes = item.Notes
                }));
                AppSession.Tables.Add(table);
            }

            _status.Text = items.Count == 0 ? "Não existem itens no inventário." : "";
            Render();
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            _status.Text = $"Não foi possível carregar o inventário: {exception.Message}";
            _status.TextColor = ViewTheme.Error;
            _retryButton.IsVisible = true;
        }
    }

    private View BuildInventoryTable(StockTable table)
    {
        var columns = new[]
        {
            new GridLength(360),
            new GridLength(140),
            new GridLength(120),
            new GridLength(140),
            new GridLength(160),
            new GridLength(240),
            new GridLength(90),
            new GridLength(90)
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
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Padding = new Thickness(20, 0),
            HeightRequest = 40,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        var panelTitle = ViewTheme.TextLabel(
            table.Name.ToUpper(AppSession.Portuguese), 11, true, ViewTheme.Muted);
        sectionHeader.Add(panelTitle, 0, 0);
        var newItemButton = ViewTheme.Action(
            "+ Novo item",
            () => EditItem(table, null),
            primary: false);
        newItemButton.WidthRequest = 120;
        newItemButton.HeightRequest = 32;
        newItemButton.MinimumHeightRequest = 0;
        newItemButton.Padding = 0;
        newItemButton.FontSize = 12;
        sectionHeader.Add(newItemButton, 1, 0);
        var sectionHeaderDivider = new BoxView
        {
            HeightRequest = 1,
            Color = ViewTheme.Stroke,
            VerticalOptions = LayoutOptions.End
        };
        sectionHeader.Add(sectionHeaderDivider, 0, 0);
        Microsoft.Maui.Controls.Grid.SetColumnSpan(sectionHeaderDivider, 2);
        panel.Children.Add(sectionHeader);

        var headings = CreateRow(
            ViewTheme.TextLabel("Item", 12, true, ViewTheme.Muted),
            CenteredLabel("Marca", ViewTheme.Muted, 12, true),
            AlignedLabel("Custo/Un.", TextAlignment.End),
            CenteredLabel("Estado", ViewTheme.Muted, 12, true),
            CenteredLabel("Função", ViewTheme.Muted, 12, true),
            CenteredLabel("Observações", ViewTheme.Muted, 12, true),
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
                AlignedLabel(ValueOrDash(item.Brand), TextAlignment.Center, ViewTheme.Muted, 12, false),
                AlignedLabel(AppSession.Money(item.UnitCost), TextAlignment.End, ViewTheme.Muted, 14),
                AlignedLabel(ValueOrDash(item.Condition), TextAlignment.Center, ViewTheme.Muted, 12, false),
                AlignedLabel(ValueOrDash(item.Function), TextAlignment.Center, ViewTheme.Muted, 12, false),
                AlignedLabel(ValueOrDash(item.Notes), TextAlignment.Start, ViewTheme.Muted, 12, false),
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

        var tableBorder = new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            WidthRequest = 1340,
            Content = panel
        };

        return new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Always,
            Content = tableBorder
        };
    }

    private static string ValueOrDash(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "—" : value;

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

        var button = new StateButton
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
        ViewTheme.BindAction(button, callback);

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

    private static int CompareTableNames(StockTable left, StockTable right)
    {
        var leftIsNumber = int.TryParse(left.Name.Trim(), out var leftNumber);
        var rightIsNumber = int.TryParse(right.Name.Trim(), out var rightNumber);

        if (leftIsNumber && rightIsNumber)
        {
            return leftNumber.CompareTo(rightNumber);
        }

        if (leftIsNumber != rightIsNumber)
        {
            return leftIsNumber ? -1 : 1;
        }

        return StringComparer.CurrentCultureIgnoreCase.Compare(left.Name, right.Name);
    }

    private async Task EditItem(StockTable table, StockItem? item)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var popup = new AddInventoryItemPopup(
            item?.Name,
            item?.Brand,
            item?.UnitCost,
            item?.Condition,
            item?.Function,
            item?.Notes);
        var draft = await ModalOverlay.ShowAsync(
            shell.CurrentPage,
            popup,
            popup.Completion,
            popup.Cancel);
        if (draft is null)
        {
            return;
        }

        ApiInventoryItem? savedItem;
        try
        {
            var request = new ApiInventoryItemRequest
            {
                Designation = draft.Name,
                InstrumentType = table.Name,
                Brand = draft.Brand,
                Price = draft.UnitCost,
                Condition = draft.Condition,
                Function = draft.Function,
                Notes = draft.Notes
            };
            if (item is null)
            {
                savedItem = await _apiClient.PostAsync<ApiInventoryItem>("api/inventory", request);
            }
            else
            {
                await _apiClient.PutAsync($"api/inventory/{item.Id}", request);
                savedItem = null;
            }
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or
            InvalidOperationException)
        {
            var message = $"Não foi possível guardar o item: {exception.Message}";
            _status.Text = message;
            _status.TextColor = ViewTheme.Error;
            await shell.DisplayAlertAsync("Inventário", message, "OK");
            return;
        }

        item ??= new StockItem();
        if (savedItem is not null)
        {
            item.Id = savedItem.Id;
        }
        item.Name = draft.Name;
        item.Brand = draft.Brand;
        item.UnitCost = draft.UnitCost;
        item.Condition = draft.Condition;
        item.Function = draft.Function;
        item.Notes = draft.Notes;
        if (!table.Items.Contains(item))
        {
            table.Items.Add(item);
        }

        await LoadInventoryAsync();
        _status.Text = $"Item '{draft.Name}' guardado na base de dados.";
        _status.TextColor = ViewTheme.Muted;
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

        try
        {
            await _apiClient.DeleteAsync($"api/inventory/{item.Id}");
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException or InvalidOperationException)
        {
            _status.Text = $"Não foi possível remover o item: {exception.Message}";
            _status.TextColor = ViewTheme.Error;
            return;
        }
        table.Items.Remove(item);
        Render();
    }
}

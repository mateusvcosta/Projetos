namespace TunaEngine.Views;

internal static class ViewTheme
{
    public static Color Background => GetColor("ThemeBackground");
    public static Color Sidebar => GetColor("ThemeSidebar");
    public static Color Surface => GetColor("ThemeSurface");
    public static Color SurfaceDark => GetColor("ThemeSurfaceDark");
    public static Color Stroke => GetColor("ThemeStroke");
    public static Color Accent => GetColor("ThemeAccent");
    public static Color Text => GetColor("ThemeText");
    public static Color Muted => GetColor("ThemeMuted");
    public static Color Error => GetColor("ThemeError");

    public static Label TextLabel(string text, double size = 14, bool bold = false, Color? color = null) =>
        new()
        {
            Text = text,
            FontSize = size,
            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
            TextColor = color ?? Text,
            VerticalTextAlignment = TextAlignment.Center
        };

    public static VerticalStackLayout Stack(params View[] children)
    {
        var stack = new VerticalStackLayout { Spacing = 16 };
        foreach (var child in children)
        {
            stack.Children.Add(child);
        }

        return stack;
    }

    public static Border Card(View content, double padding = 20) =>
        new()
        {
            BackgroundColor = Surface,
            Stroke = Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = new Thickness(padding),
            Content = content
        };

    public static Border Metric(
        string label,
        string value,
        double labelSize = 11,
        double valueSize = 24,
        double padding = 20,
        Color? valueColor = null)
    {
        var content = Stack(
            TextLabel(label, labelSize, color: Muted),
            TextLabel(value, valueSize, true, valueColor));
        content.Spacing = 6;
        return Card(content, padding);
    }

    public static Border EventCard(string title, string description)
    {
        var row = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(4)),
                new ColumnDefinition(GridLength.Star)
            }
        };
        row.Add(new BoxView
        {
            Color = Accent,
            WidthRequest = 4,
            VerticalOptions = LayoutOptions.Fill
        }, 0, 0);

        var copy = Stack(
            TextLabel(title, 15, true),
            TextLabel(description, 13, color: Muted));
        copy.Spacing = 4;
        copy.Padding = new Thickness(16, 14);
        copy.VerticalOptions = LayoutOptions.Center;
        row.Add(copy, 1, 0);
        return new Border
        {
            BackgroundColor = Surface,
            Stroke = Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            HeightRequest = 68,
            Padding = 0,
            Content = row
        };
    }

    public static View Divider() => new BoxView
    {
        HeightRequest = 1,
        Color = Stroke
    };

    public static Grid Row(double[] widths, params View[] cells)
    {
        var row = new Grid
        {
            Padding = new Thickness(16, 12),
            ColumnSpacing = 12
        };
        foreach (var width in widths)
        {
            row.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(width)));
        }

        for (var index = 0; index < cells.Length; index++)
        {
            row.Add(cells[index], index, 0);
        }

        return row;
    }

    public static View Table(double width, double[] columns, string[] headings, IEnumerable<View> rows)
    {
        var table = new VerticalStackLayout
        {
            Spacing = 0,
            WidthRequest = width
        };

        var headingLabels = headings
            .Select(text => (View)TextLabel(text, 12, true, Muted))
            .ToArray();
        var header = Row(columns, headingLabels);
        header.BackgroundColor = SurfaceDark;
        table.Children.Add(header);

        foreach (var row in rows)
        {
            table.Children.Add(Divider());
            table.Children.Add(row);
        }

        return new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Content = Card(table, 0)
        };
    }

    public static Button Action(string text, Func<Task> callback, bool primary = true)
    {
        var button = new Button
        {
            Text = text,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = primary ? Accent : Surface,
            TextColor = Text,
            CornerRadius = 10,
            Padding = new Thickness(18, 10),
            MinimumHeightRequest = 44
        };
        button.Clicked += async (_, _) => await callback();
        return button;
    }

    private static Color GetColor(string key) =>
        (Color)Application.Current!.Resources[key];
}

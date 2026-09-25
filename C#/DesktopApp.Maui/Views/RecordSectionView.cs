namespace DesktopApp.Maui.Views;

public abstract class RecordSectionView : ContentView
{
    protected RecordSectionView(string title, string description, string heading, string[] records, bool isWideLayout)
    {
        var content = new VerticalStackLayout { Spacing = 18 };
        var titleLabel = new Label
        {
            Text = title,
            FontSize = isWideLayout ? 34 : 29,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F3F5F4")
        };
        SemanticProperties.SetHeadingLevel(titleLabel, SemanticHeadingLevel.Level1);
        content.Children.Add(titleLabel);
        content.Children.Add(new Label
        {
            Text = description,
            FontSize = 15,
            TextColor = Color.FromArgb("#AAB7BA")
        });
        content.Children.Add(new Label
        {
            Text = heading,
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            CharacterSpacing = 1.5,
            TextColor = Color.FromArgb("#91A0A5"),
            Margin = new Thickness(0, 12, 0, 0)
        });

        foreach (var record in records)
        {
            content.Children.Add(CreateRecord(record));
        }

        Content = content;
    }

    private static Border CreateRecord(string text)
    {
        var content = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 16,
            Padding = new Thickness(16, 15)
        };
        content.Add(new Border
        {
            WidthRequest = 4,
            BackgroundColor = Color.FromArgb("#D94F65"),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 2 }
        }, 0, 0);
        content.Add(new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new Label
                {
                    Text = text,
                    FontSize = 17,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#F3F5F4")
                },
                new Label
                {
                    Text = "Conteúdo de demonstração",
                    FontSize = 13,
                    TextColor = Color.FromArgb("#91A0A5")
                }
            }
        }, 1, 0);

        return new Border
        {
            BackgroundColor = Color.FromArgb("#1A262E"),
            Stroke = Color.FromArgb("#2D3A42"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = content
        };
    }
}
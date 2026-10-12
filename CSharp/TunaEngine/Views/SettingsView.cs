namespace TunaEngine.Views;

public sealed class SettingsView : ContentView
{
    public SettingsView()
    {
        var content = new VerticalStackLayout { Spacing = 16 };
        var title = ViewTheme.TextLabel("Definições", 30, true);
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        content.Children.Add(title);
        content.Children.Add(ViewTheme.TextLabel(
            "Preferências da aplicação.",
            14,
            color: ViewTheme.Muted));

        content.Children.Add(CreateSectionLabel("Registos"));
        foreach (var setting in new[]
                 {
                     new { Name = "Tema", Description = "Conteúdo de demonstração" },
                     new { Name = "Notificações", Description = "Conteúdo de demonstração" },
                     new { Name = "Segurança", Description = "Conteúdo de demonstração" }
                 })
        {
            content.Children.Add(CreateSettingRow(setting.Name, setting.Description, () => ShowSettingInfo(setting.Name)));
        }

        content.Children.Add(CreateSectionLabel("Zona de perigo"));
        content.Children.Add(CreateDangerRow("Terminar Sessão", ShowSignOutInfo));
        content.Children.Add(ViewTheme.TextLabel(
            "TunaEngine · Versão de demonstração",
            12,
            color: Color.FromArgb("#444466")));

        Content = content;
    }

    private static View CreateSectionLabel(string text) => new Label
    {
        Text = text,
        TextTransform = TextTransform.Uppercase,
        FontSize = 11,
        FontAttributes = FontAttributes.Bold,
        TextColor = ViewTheme.Muted,
        Margin = new Thickness(0, 16, 0, 0)
    };

    private static Border CreateSettingRow(string title, string description, Func<Task> callback)
    {
        var content = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(4)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(18))
            },
            Padding = 0,
            HeightRequest = 72,
            ColumnSpacing = 0
        };

        content.Add(new BoxView
        {
            Color = ViewTheme.Accent,
            WidthRequest = 4,
            HeightRequest = 72,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        }, 0, 0);

        var textLayout = new VerticalStackLayout
        {
            Spacing = 4,
            Padding = new Thickness(16),
            VerticalOptions = LayoutOptions.Center
        };
        textLayout.Children.Add(ViewTheme.TextLabel(title, 15, true, ViewTheme.Text));
        textLayout.Children.Add(ViewTheme.TextLabel(description, 13, false, ViewTheme.Muted));
        content.Add(textLayout, 1, 0);

        var chevron = new Label
        {
            Text = ">",
            FontSize = 18,
            TextColor = ViewTheme.Muted,
            HorizontalTextAlignment = TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 20, 0)
        };
        content.Add(chevron, 2, 0);

        var button = ViewTheme.Action("", callback, false);
        button.BorderWidth = 0;
        button.BackgroundColor = Colors.Transparent;
        button.TextColor = Colors.Transparent;
        button.Margin = 0;
        button.Padding = 0;
        button.CornerRadius = 0;
        button.HorizontalOptions = LayoutOptions.Fill;
        button.VerticalOptions = LayoutOptions.Fill;

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Padding = 0,
            Content = new Grid
            {
                Children =
                {
                    content,
                    button
                }
            }
        };
    }

    private static Border CreateDangerRow(string title, Func<Task> callback)
    {
        var row = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(18)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(18))
            },
            Padding = new Thickness(20, 0),
            HeightRequest = 56,
            BackgroundColor = Colors.Transparent
        };

        var icon = new Image
        {
            Source = "logout.svg",
            WidthRequest = 18,
            HeightRequest = 18,
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        row.Add(icon, 0, 0);

        var label = ViewTheme.TextLabel(title, 15, true, ViewTheme.Accent);
        label.VerticalTextAlignment = TextAlignment.Center;
        row.Add(label, 1, 0);

        var chevron = new Label
        {
            Text = ">",
            FontSize = 18,
            TextColor = ViewTheme.Accent,
            HorizontalTextAlignment = TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center
        };
        row.Add(chevron, 2, 0);

        var button = ViewTheme.Action("", callback, false);
        button.BorderWidth = 0;
        button.BackgroundColor = Colors.Transparent;
        button.TextColor = Colors.Transparent;
        button.Margin = 0;
        button.Padding = 0;
        button.CornerRadius = 0;
        button.HorizontalOptions = LayoutOptions.Fill;
        button.VerticalOptions = LayoutOptions.Fill;

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Accent,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Padding = 0,
            Content = new Grid
            {
                Children =
                {
                    row,
                    button
                }
            }
        };
    }

    private static async Task ShowSettingInfo(string setting)
    {
        var shell = Shell.Current;
        if (shell is not null)
        {
            await shell.DisplayAlertAsync(
                setting,
                "Esta preferência é demonstrativa. Ligue-a ao serviço correspondente da aplicação.",
                "OK");
        }
    }

    private static async Task ShowSignOutInfo()
    {
        var shell = Shell.Current;
        if (shell is not null)
        {
            await shell.DisplayAlertAsync(
                "Sessão de demonstração",
                "Não existe uma sessão autenticada nesta implementação.",
                "OK");
        }
    }
}

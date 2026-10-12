namespace TunaEngine.Views;

public sealed class ProfileView : ContentView
{
    private readonly Label _nameValue = ViewTheme.TextLabel(AppSession.ProfileName, 14, false, Color.FromArgb("#CCCCDD"));
    private readonly Label _emailValue = ViewTheme.TextLabel(AppSession.ProfileEmail, 14, false, Color.FromArgb("#CCCCDD"));
    private readonly Label _roleValue = ViewTheme.TextLabel(AppSession.ProfileRole, 14, false, Color.FromArgb("#CCCCDD"));

    public ProfileView()
    {
        var content = new VerticalStackLayout { Spacing = 24 };
        content.Children.Add(CreateHeroCard());

        var summaryGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(1.2, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star))
            },
            ColumnSpacing = 24
        };

        summaryGrid.Add(CreateDetailsCard(), 0, 0);
        summaryGrid.Add(CreateActivityCard(), 1, 0);
        content.Children.Add(summaryGrid);

        Content = content;
    }

    private async Task EditProfile()
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var name = await shell.DisplayPromptAsync(
            "Editar perfil", "Nome:", "Continuar", "Cancelar", initialValue: AppSession.ProfileName);
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var email = await shell.DisplayPromptAsync(
            "Editar perfil", "Email:", "Continuar", "Cancelar",
            keyboard: Keyboard.Email, initialValue: AppSession.ProfileEmail);
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var role = await shell.DisplayPromptAsync(
            "Editar perfil", "Cargo na equipa:", "Guardar", "Cancelar", initialValue: AppSession.ProfileRole);
        if (string.IsNullOrWhiteSpace(role))
        {
            return;
        }

        AppSession.ProfileName = name.Trim();
        AppSession.ProfileEmail = email.Trim();
        AppSession.ProfileRole = role.Trim();
        _nameValue.Text = AppSession.ProfileName;
        _emailValue.Text = AppSession.ProfileEmail;
        _roleValue.Text = AppSession.ProfileRole;
    }

    private Border CreateHeroCard()
    {
        var avatar = new Border
        {
            WidthRequest = 96,
            HeightRequest = 96,
            BackgroundColor = ViewTheme.Accent,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 48 }
        };

        var heroText = new VerticalStackLayout { Spacing = 6 };
        heroText.Children.Add(ViewTheme.TextLabel("Perfil de demonstração", 26, true, ViewTheme.Text));
        heroText.Children.Add(ViewTheme.TextLabel("Conteúdo de demonstração · sem dados reais", 14, color: ViewTheme.Muted));

        var chips = new HorizontalStackLayout { Spacing = 8, Margin = new Thickness(0, 1, 0, 0) };
        chips.Children.Add(CreateStatusChip("Demonstração", "#1A3A2A", "#4ADE80"));
        chips.Children.Add(ViewTheme.TextLabel("Sem dados reais", 12, color: Color.FromArgb("#A3A3B8")));
        heroText.Children.Add(chips);

        var heroLayout = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 32,
            Padding = new Thickness(32)
        };

        heroLayout.Add(avatar, 0, 0);
        heroLayout.Add(heroText, 1, 0);

        var editButton = ViewTheme.Action("Editar Perfil", EditProfile, true);
        editButton.WidthRequest = 120;
        editButton.HeightRequest = 40;
        editButton.FontSize = 13;
        editButton.CornerRadius = 8;
        editButton.Padding = new Thickness(0);
        editButton.BackgroundColor = ViewTheme.Accent;
        editButton.TextColor = Colors.White;
        editButton.HorizontalOptions = LayoutOptions.End;
        Grid.SetColumn(editButton, 2);
        heroLayout.Add(editButton);

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = heroLayout
        };
    }

    private Border CreateDetailsCard()
    {
        var content = new VerticalStackLayout { Spacing = 0 };
        content.Children.Add(CreateSectionHeader("Informação Pessoal"));
        content.Children.Add(CreateFieldRow("Nome", _nameValue));
        content.Children.Add(ViewTheme.Divider());
        content.Children.Add(CreateFieldRow("Email", _emailValue));
        content.Children.Add(ViewTheme.Divider());
        content.Children.Add(CreateFieldRow("Cargo na equipa", _roleValue));

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = content
        };
    }

    private Border CreateActivityCard()
    {
        var content = new VerticalStackLayout { Spacing = 0 };
        content.Children.Add(CreateSectionHeader("Atividade Recente"));

        var empty = new VerticalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(0, 24),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HeightRequest = 200
        };

        var icon = new Border
        {
            WidthRequest = 40,
            HeightRequest = 40,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Padding = new Thickness(10),
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Center
        };
        icon.Content = new Label
        {
            Text = "◌",
            FontSize = 18,
            TextColor = ViewTheme.Muted,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        empty.Children.Add(icon);
        empty.Children.Add(ViewTheme.TextLabel("Sem atividade recente", 14, color: Color.FromArgb("#A3A3B8")));
        empty.Children.Add(ViewTheme.TextLabel("Conteúdo de demonstração", 12, color: Color.FromArgb("#444466")));
        content.Children.Add(empty);

        return new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = content
        };
    }

    private static Grid CreateSectionHeader(string title)
    {
        var header = new Grid
        {
            Padding = new Thickness(20, 0),
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent
        };

        var label = ViewTheme.TextLabel(title, 11, true, ViewTheme.Muted);
        label.TextTransform = TextTransform.Uppercase;
        header.Add(label);
        return header;
    }

    private static Grid CreateFieldRow(string labelText, Label valueLabel)
    {
        var row = new Grid
        {
            Padding = new Thickness(20, 0),
            HeightRequest = 56,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            BackgroundColor = Colors.Transparent
        };

        var label = ViewTheme.TextLabel(labelText, 14, false, ViewTheme.Muted);
        valueLabel.FontSize = 14;
        valueLabel.TextColor = Color.FromArgb("#CCCCDD");
        valueLabel.HorizontalTextAlignment = TextAlignment.End;
        valueLabel.VerticalTextAlignment = TextAlignment.Center;

        row.Add(label, 0, 0);
        row.Add(valueLabel, 1, 0);
        return row;
    }

    private static Border CreateStatusChip(string text, string bg, string fg)
    {
        var label = ViewTheme.TextLabel(text, 11, true, Color.FromArgb(fg));
        label.VerticalTextAlignment = TextAlignment.Center;
        label.HorizontalTextAlignment = TextAlignment.Center;

        return new Border
        {
            BackgroundColor = Color.FromArgb(bg),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            Padding = new Thickness(12, 6),
            Content = label,
            HeightRequest = 26
        };
    }
}

using TunaEngine.Views;
using Microsoft.Maui.Controls.Shapes;

namespace TunaEngine;

public sealed class MainPage : ContentPage
{
    private sealed record NavigationSection(string Title, string Label, string? Icon, string? ActiveIcon);

    private static readonly NavigationSection[] Sections =
    [
        new("Início", "Início", "home.png", "home_active.png"),
        new("Inventário", "Inventário", "inventory.png", "inventory_active.png"),
        new("Eventos", "Eventos", "events.png", "events_active.png"),
        new("Membros", "Membros", "members.png", "members_active.png"),
        new("Perfil", "Perfil", "profile.png", "profile_active.png"),
        new("Definições", "Definições", "settings.png", "settings_active.png")
    ];

    private readonly List<(NavigationSection Section, Border Background, Button Button, Image Icon, Label Label)> navigationItems = [];
    private readonly Grid layoutRoot;
    private Border navigationPanel = null!;
    private VerticalStackLayout navigationItemsLayout = null!;
    private Grid headerContentGrid = null!;
    private Button menuButton = null!;
    private Label pageTitleLabel = null!;
    private Grid searchBorder = null!;
    private Button notificationsButton = null!;
    private VerticalStackLayout pageContent = null!;
    private string currentSection = "Início";
    private bool isWideLayout;
    private bool isMenuVisible;
    private bool hasAppliedLayout;

    public MainPage()
    {
        BackgroundColor = ViewTheme.Background;
        Shell.SetNavBarIsVisible(this, false);

        layoutRoot = CreateLayout();
        BuildNavigation();
        menuButton.Clicked += (_, _) =>
        {
            isMenuVisible = !isMenuVisible;
            UpdateLayoutForWidth();
        };
        notificationsButton.Clicked += async (_, _) =>
            await DisplayAlertAsync("Notificações", "Não existem notificações novas.", "OK");
        ShowSection(currentSection);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var wideLayout = width >= 760;
        if (wideLayout == isWideLayout && hasAppliedLayout)
        {
            return;
        }

        isMenuVisible = wideLayout;

        isWideLayout = wideLayout;
        hasAppliedLayout = true;
        UpdateLayoutForWidth();
        ShowSection(currentSection);
    }

    private void BuildNavigation()
    {
        foreach (var section in Sections)
        {
            var item = CreateNavigationButton(section);
            navigationItems.Add((section, item.Background, item.Button, item.Icon, item.Label));
            navigationItemsLayout.Children.Add(item.Container);
        }
    }

    private (Grid Container, Border Background, Button Button, Image Icon, Label Label)
        CreateNavigationButton(NavigationSection section)
    {
        var icon = new Image
        {
            Source = section.Icon,
            WidthRequest = 18,
            HeightRequest = 18,
            Aspect = Aspect.AspectFit,
            VerticalOptions = LayoutOptions.Center,
            InputTransparent = true
        };
        var label = new Label
        {
            Text = section.Label,
            FontSize = 14,
            TextColor = ViewTheme.Muted,
            VerticalTextAlignment = TextAlignment.Center,
            InputTransparent = true
        };
        var content = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(18)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 12,
            Padding = new Thickness(12, 0),
            InputTransparent = true
        };
        content.Add(icon, 0, 0);
        content.Add(label, 1, 0);

        var background = new Border
        {
            HeightRequest = 44,
            Padding = 0,
            BackgroundColor = Colors.Transparent,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            InputTransparent = true
        };
        var button = new Button
        {
            Text = string.Empty,
            AutomationId = $"Navigate{section.Title}",
            HeightRequest = 44,
            Padding = 0,
            BackgroundColor = Colors.Transparent,
            BorderWidth = 0,
            MinimumHeightRequest = 0,
            MinimumWidthRequest = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Opacity = 0
        };
        SemanticProperties.SetDescription(button, $"Navegar para {section.Title}");
        button.Clicked += (_, _) => ShowSection(section.Title);

        var container = new Grid
        {
            HeightRequest = 44,
            HorizontalOptions = LayoutOptions.Fill
        };
        container.Add(background);
        container.Add(content);
        container.Add(button);
        return (container, background, button, icon, label);
    }

    private Grid CreateLayout()
    {
        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(new GridLength(72)),
                new RowDefinition(GridLength.Star)
            },
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(220)),
                new ColumnDefinition(GridLength.Star)
            }
        };

        var brand = new Grid { BackgroundColor = ViewTheme.Sidebar };
        var brandLayout = new Grid
        {
            Padding = new Thickness(24, 0),
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(32)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10
        };
        brandLayout.Add(new Border
        {
            WidthRequest = 32,
            HeightRequest = 32,
            BackgroundColor = ViewTheme.Accent,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = "T",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }
        }, 0, 0);
        brandLayout.Add(new Label
        {
            Text = "TunaEngine",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            VerticalTextAlignment = TextAlignment.Center
        }, 1, 0);
        brand.Add(brandLayout);
        root.Add(brand, 0, 0);

        navigationItemsLayout = new VerticalStackLayout { Spacing = 4 };
        var navigationScroll = new ScrollView
        {
            Orientation = ScrollOrientation.Vertical,
            Padding = new Thickness(0, 16, 0, 0),
            VerticalOptions = LayoutOptions.Fill,
            Content = navigationItemsLayout
        };
        var accountLayout = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(28)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10
        };
        accountLayout.Add(new Ellipse
        {
            WidthRequest = 28,
            HeightRequest = 28,
            Fill = ViewTheme.Accent,
            VerticalOptions = LayoutOptions.Center
        }, 0, 0);
        accountLayout.Add(CreateAccountLabels(), 1, 0);
        var account = new Border
        {
            Margin = new Thickness(0, 0, 0, 12),
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(12, 0),
            Content = accountLayout
        };
        var navigationContent = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(new GridLength(44))
            }
        };
        navigationContent.Add(navigationScroll, 0, 0);
        navigationContent.Add(account, 0, 1);
        navigationPanel = new Border
        {
            BackgroundColor = ViewTheme.Sidebar,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 0,
            StrokeShape = new Rectangle(),
            Padding = new Thickness(12, 0),
            IsVisible = false,
            Content = navigationContent
        };
        root.Add(navigationPanel, 0, 1);

        var header = new Grid { BackgroundColor = ViewTheme.Sidebar };
        headerContentGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(640)),
                new ColumnDefinition(new GridLength(260)),
                new ColumnDefinition(new GridLength(20)),
                new ColumnDefinition(new GridLength(36))
            },
            ColumnSpacing = 0,
            WidthRequest = 956,
            Margin = new Thickness(40, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Start
        };
        menuButton = new Button
        {
            Text = "☰",
            FontSize = 16,
            BackgroundColor = Colors.Transparent,
            TextColor = ViewTheme.Text,
            Padding = 4,
            MinimumWidthRequest = 32,
            MinimumHeightRequest = 32,
            IsVisible = false
        };
        pageTitleLabel = new Label
        {
            Text = "Início",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = ViewTheme.Text,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = 0
        };
        searchBorder = CreateSearch();
        var notifications = new Grid
        {
            WidthRequest = 36,
            HeightRequest = 36,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        notifications.Add(new Border
        {
            BackgroundColor = ViewTheme.Surface,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 8 }
        });
        notifications.Add(new Image
        {
            Source = "bell.png",
            WidthRequest = 22,
            HeightRequest = 22,
            Aspect = Aspect.AspectFit,
            InputTransparent = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        });
        notificationsButton = new Button
        {
            Text = string.Empty,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            Margin = 0,
            MinimumWidthRequest = 0,
            MinimumHeightRequest = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            AutomationId = "Notifications"
        };
        SemanticProperties.SetDescription(notificationsButton, "Notificações");
        notifications.Add(notificationsButton);

        headerContentGrid.Add(menuButton, 0, 0);
        headerContentGrid.Add(pageTitleLabel, 0, 0);
        headerContentGrid.Add(searchBorder, 1, 0);
        headerContentGrid.Add(notifications, 3, 0);
        header.Add(headerContentGrid);
        root.Add(header, 1, 0);

        var divider = new BoxView
        {
            HeightRequest = 1,
            Color = ViewTheme.Stroke,
            VerticalOptions = LayoutOptions.End,
            InputTransparent = true
        };
        root.Add(divider);
        Grid.SetRow(divider, 0);
        Grid.SetColumnSpan(divider, 2);

        pageContent = new VerticalStackLayout
        {
            Padding = new Thickness(40, 32, 40, 32),
            Spacing = 0
        };
        var contentScroll = new ScrollView { Content = pageContent };
        root.Add(contentScroll, 1, 1);
        Content = root;
        return root;
    }

    private static VerticalStackLayout CreateAccountLabels()
    {
        var labels = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
        labels.Children.Add(new Label
        {
            Text = "Utilizador",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = ViewTheme.Text
        });
        labels.Children.Add(new Label
        {
            Text = "Membro",
            FontSize = 11,
            TextColor = ViewTheme.Muted
        });
        return labels;
    }

    private static Grid CreateSearch()
    {
        var search = new Grid
        {
            WidthRequest = 260,
            HeightRequest = 36,
            VerticalOptions = LayoutOptions.Center
        };
        var entry = new Entry
        {
            AutomationId = "SearchEntry",
            Placeholder = "Pesquisar...",
            FontSize = 13,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center,
            HeightRequest = 34,
            MinimumHeightRequest = 0,
            MinimumWidthRequest = 0,
            Margin = 0
        };
        var searchContents = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(16)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 8,
            VerticalOptions = LayoutOptions.Center
        };
        searchContents.Add(new Label
        {
            Text = "⌕",
            FontSize = 16,
            TextColor = ViewTheme.Muted,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center
        }, 0, 0);
        searchContents.Add(entry, 1, 0);
        search.Add(new Border
        {
            HeightRequest = 36,
            Padding = new Thickness(12, 0),
            BackgroundColor = ViewTheme.SurfaceDark,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            VerticalOptions = LayoutOptions.Center,
            Content = searchContents
        });
        return search;
    }

    private void UpdateLayoutForWidth()
    {
        if (isWideLayout)
        {
            navigationPanel.Padding = new Thickness(12, 0);
        }
        else
        {
            navigationPanel.Padding = new Thickness(10, 0);
        }

        var showSidebar = isMenuVisible;
        layoutRoot.ColumnDefinitions[0].Width = new GridLength(
            showSidebar ? (isWideLayout ? 220 : 170) : 0);
        layoutRoot.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        navigationPanel.IsVisible = showSidebar;
        navigationItemsLayout.HorizontalOptions = LayoutOptions.Fill;
        pageContent.Padding = new Thickness(
            isWideLayout ? 40 : 16,
            isWideLayout ? 32 : 24,
            isWideLayout ? 40 : 16,
            isWideLayout ? 18 : 24);

        foreach (var item in navigationItems)
        {
            item.Background.WidthRequest = isWideLayout ? 196 : 150;
            item.Button.HeightRequest = 44;
            item.Background.HorizontalOptions = LayoutOptions.Fill;
        }

        headerContentGrid.WidthRequest = isWideLayout ? 956 : -1;
        headerContentGrid.ColumnDefinitions[0].Width = isWideLayout
            ? new GridLength(640)
            : new GridLength(40);
        headerContentGrid.ColumnDefinitions[1].Width = isWideLayout
            ? new GridLength(260)
            : GridLength.Star;
        headerContentGrid.ColumnDefinitions[2].Width = isWideLayout
            ? new GridLength(20)
            : new GridLength(0);
        headerContentGrid.ColumnDefinitions[3].Width = isWideLayout
            ? new GridLength(36)
            : new GridLength(0);
        Grid.SetColumn(menuButton, 0);
        Grid.SetColumn(pageTitleLabel, isWideLayout ? 0 : 1);
        menuButton.IsVisible = !isWideLayout;
        searchBorder.IsVisible = isWideLayout;
        notificationsButton.IsVisible = isWideLayout;
    }

    private void ShowSection(string section)
    {
        currentSection = section;
        pageTitleLabel.Text = section;
        foreach (var item in navigationItems)
        {
            var selected = item.Section.Title == section;
            item.Background.BackgroundColor = selected ? ViewTheme.Accent : Colors.Transparent;
            item.Label.TextColor = selected ? ViewTheme.Text : ViewTheme.Muted;
            item.Label.FontAttributes = selected ? FontAttributes.Bold : FontAttributes.None;
            var iconFile = selected
                ? item.Section.ActiveIcon ?? item.Section.Icon
                : item.Section.Icon;
            if (iconFile is not null)
                item.Icon.Source = iconFile;
        }

        ContentView sectionView = section switch
        {
            "Inventário" => new InventoryView(),
            "Eventos" => new EventsView(),
            "Membros" => new MembersView(),
            "Perfil" => new ProfileView(),
            "Definições" => new SettingsView(),
            _ => new HomeView(isWideLayout)
        };

        pageContent.Children.Clear();
        pageContent.Children.Add(sectionView);

        if (!isWideLayout && isMenuVisible)
        {
            isMenuVisible = false;
            UpdateLayoutForWidth();
        }
    }
}

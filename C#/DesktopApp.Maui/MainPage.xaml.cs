using DesktopApp.Maui.Views;

namespace DesktopApp.Maui;

public partial class MainPage : ContentPage
{
    private static readonly string[] Sections =
    [
        "Início",
        "Inventário",
        "Calendário",
        "Membros",
        "Definições"
    ];

    private string currentSection = "Início";
    private bool isWideLayout;

    public MainPage()
    {
        InitializeComponent();
        BuildNavigation();
        ShowSection(currentSection);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var wideLayout = width >= 760;
        if (wideLayout == isWideLayout)
        {
            return;
        }

        isWideLayout = wideLayout;
        UpdateLayoutForWidth();
        ShowSection(currentSection);
    }

    private void BuildNavigation()
    {
        foreach (var section in Sections)
        {
            var button = new Button
            {
                Text = section,
                Style = (Style)Application.Current!.Resources["NavigationButton"],
                AutomationId = $"Navigate{section.Replace(" ", string.Empty)}"
            };
            button.Clicked += (_, _) => ShowSection(section);
            NavigationItems.Children.Add(button);
        }
    }

    private void UpdateLayoutForWidth()
    {
        if (isWideLayout)
        {
            LayoutRoot.ColumnDefinitions[0].Width = new GridLength(248);
            LayoutRoot.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions[1].Height = new GridLength(0);
            Grid.SetRow(NavigationPanel, 0);
            Grid.SetRowSpan(NavigationPanel, 2);
            NavigationPanel.StrokeThickness = 1;
            NavigationPanel.Padding = new Thickness(18, 28);
            NavigationContent.VerticalOptions = LayoutOptions.Fill;
            BrandBlock.IsVisible = true;
            NavigationScroll.Orientation = ScrollOrientation.Vertical;
            NavigationScroll.HeightRequest = -1;
            NavigationScroll.VerticalOptions = LayoutOptions.Fill;
            NavigationItems.Orientation = StackOrientation.Vertical;
            NavigationItems.HorizontalOptions = LayoutOptions.Fill;
            PageContent.Padding = new Thickness(48, 40);
            Grid.SetColumn(ContentScroll, 1);
        }
        else
        {
            LayoutRoot.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions[1].Width = new GridLength(0);
            LayoutRoot.RowDefinitions[1].Height = GridLength.Auto;
            Grid.SetRow(NavigationPanel, 1);
            Grid.SetRowSpan(NavigationPanel, 1);
            NavigationPanel.StrokeThickness = 1;
            NavigationPanel.Padding = new Thickness(8, 6);
            NavigationContent.VerticalOptions = LayoutOptions.Start;
            BrandBlock.IsVisible = false;
            NavigationScroll.Orientation = ScrollOrientation.Horizontal;
            NavigationScroll.HeightRequest = 54;
            NavigationScroll.VerticalOptions = LayoutOptions.Start;
            NavigationItems.Orientation = StackOrientation.Horizontal;
            NavigationItems.HorizontalOptions = LayoutOptions.Start;
            PageContent.Padding = new Thickness(22, 26);
            Grid.SetColumn(ContentScroll, 0);
        }

        foreach (var child in NavigationItems.Children.OfType<Button>())
        {
            child.WidthRequest = isWideLayout ? 208 : 92;
            child.HorizontalOptions = isWideLayout ? LayoutOptions.Fill : LayoutOptions.Start;
            child.Padding = isWideLayout ? new Thickness(14, 12) : new Thickness(7, 10);
        }
    }

    private void ShowSection(string section)
    {
        currentSection = section;
        foreach (var button in NavigationItems.Children.OfType<Button>())
        {
            var selected = button.Text == section;
            button.BackgroundColor = selected ? Color.FromArgb("#D94F65") : Colors.Transparent;
            button.TextColor = selected ? Colors.White : Color.FromArgb("#AAB7BA");
        }

        ContentView sectionView = section switch
        {
            "Inventário" => new InventoryView(isWideLayout),
            "Calendário" => new CalendarView(isWideLayout),
            "Membros" => new MembersView(isWideLayout),
            "Definições" => new SettingsView(isWideLayout),
            _ => new HomeView(isWideLayout)
        };

        PageContent.Children.Clear();
        PageContent.Children.Add(sectionView);
    }
}

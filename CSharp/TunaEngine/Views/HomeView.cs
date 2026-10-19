using Microsoft.Maui.Layouts;
using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;

namespace TunaEngine.Views;

public sealed class HomeView : ContentView
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Label _status = ViewTheme.TextLabel("A carregar dados...", 13, color: ViewTheme.Muted);
    private readonly Button _retryButton;
    private readonly VerticalStackLayout _eventList = new() { Spacing = 12, Margin = new Thickness(0, 11, 0, 0) };
    private readonly Label _upcomingCount;
    private readonly Label _memberCount;
    private readonly Label _inventoryCount;

    public HomeView(bool isWideLayout, TunaEngineApiClient apiClient)
    {
        _apiClient = apiClient;
        _retryButton = ViewTheme.Action("Tentar novamente", LoadDashboardAsync, primary: false);
        _retryButton.IsVisible = false;

        var content = new VerticalStackLayout { Spacing = 0 };
        var welcome = ViewTheme.Stack(
            ViewTheme.TextLabel("Bem-vindo ao TunaEngine 🎵", 26, true),
            ViewTheme.TextLabel(
                "Gere a tua tuna académica — inventário, eventos, membros e muito mais.",
                14,
                color: ViewTheme.Muted));
        welcome.Spacing = 8;
        var welcomeCard = ViewTheme.Card(welcome, 16);
        welcomeCard.Padding = new Thickness(32, 28);
        welcomeCard.HeightRequest = 120;
        content.Children.Add(welcomeCard);

        var cards = new[]
        {
            ViewTheme.Metric("PRÓXIMOS EVENTOS", "—", 12, 28, 24, ViewTheme.Accent),
            ViewTheme.Metric("MEMBROS ATIVOS", "—", 12, 28, 24),
            ViewTheme.Metric(
                "ITENS EM INVENTÁRIO",
                "—", 12, 28, 24),
            ViewTheme.Metric(
                "DATA SELECIONADA",
                AppSession.FormatShortDate(AppSession.SelectedDate), 12, 16, 24)
        };
        _upcomingCount = (Label)((VerticalStackLayout)cards[0].Content!).Children[1];
        _memberCount = (Label)((VerticalStackLayout)cards[1].Content!).Children[1];
        _inventoryCount = (Label)((VerticalStackLayout)cards[2].Content!).Children[1];
        foreach (var card in cards)
        {
            card.HeightRequest = 88;
        }

        if (isWideLayout)
        {
            var metrics = new Grid
            {
                ColumnSpacing = 24,
                Margin = new Thickness(0, 24, 0, 0),
                HeightRequest = 88,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(264, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(264, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(264, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(276, GridUnitType.Star))
                }
            };
            for (var index = 0; index < cards.Length; index++)
                metrics.Add(cards[index], index, 0);
            content.Children.Add(metrics);
        }
        else
        {
            var metrics = new FlexLayout
            {
                Direction = FlexDirection.Row,
                Wrap = FlexWrap.Wrap,
                Margin = new Thickness(0, 24, 0, 0)
            };
            foreach (var card in cards)
            {
                card.WidthRequest = 264;
                card.Margin = new Thickness(0, 0, 12, 0);
                metrics.Children.Add(card);
            }

            content.Children.Add(metrics);
        }

        var agendaHeading = new VerticalStackLayout { Spacing = 0 };
        var agendaTitle = ViewTheme.TextLabel("Próximos Eventos", 18, true);
        agendaTitle.HeightRequest = 22;
        agendaHeading.Children.Add(agendaTitle);
        var agendaSubtitle = ViewTheme.TextLabel(
            "Acompanhe as próximas atividades.", 13, color: ViewTheme.Muted);
        agendaSubtitle.HeightRequest = 16;
        agendaSubtitle.Margin = new Thickness(0, 6, 0, 0);
        agendaHeading.Children.Add(agendaSubtitle);
        agendaHeading.Children.Add(new Label
        {
            Text = "AGENDA",
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            TextColor = ViewTheme.Muted,
            HeightRequest = 13,
            Margin = new Thickness(0, 20, 0, 0)
        });

        _eventList.Children.Add(_status);
        _eventList.Children.Add(_retryButton);

        var agenda = new VerticalStackLayout { Spacing = 0 };
        agenda.Children.Add(agendaHeading);
        agenda.Children.Add(_eventList);

        var calendar = new SessionCalendarView(() =>
            ((Label)((VerticalStackLayout)cards[3].Content!).Children[1]).Text =
                AppSession.FormatShortDate(AppSession.SelectedDate),
            isWideLayout);
        if (isWideLayout)
        {
            var dashboard = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(700, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(404, GridUnitType.Star))
                },
                ColumnSpacing = 36,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 32, 0, 0)
            };
            dashboard.Add(agenda, 0, 0);
            dashboard.Add(calendar, 1, 0);
            content.Children.Add(dashboard);
        }
        else
        {
            var dashboard = new VerticalStackLayout
            {
                Spacing = 24,
                Margin = new Thickness(0, 32, 0, 0)
            };
            dashboard.Children.Add(agenda);
            dashboard.Children.Add(calendar);
            content.Children.Add(dashboard);
        }

        Content = content;
        _ = LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        _status.Text = "A carregar dados...";
        _status.TextColor = ViewTheme.Muted;
        _retryButton.IsVisible = false;

        try
        {
            var eventsTask = _apiClient.GetAsync<List<ApiEvent>>("api/events");
            var membersTask = _apiClient.GetAsync<List<ApiMember>>("api/members");
            var inventoryTask = _apiClient.GetAsync<List<ApiInventoryItem>>("api/inventory");
            await Task.WhenAll(eventsTask, membersTask, inventoryTask);

            var events = await eventsTask;
            var members = await membersTask;
            var inventory = await inventoryTask;
            var upcomingEvents = events
                .Select(entry => new
                {
                    Event = entry,
                    Date = entry.Date.Kind == DateTimeKind.Utc
                        ? entry.Date.ToLocalTime()
                        : entry.Date
                })
                .Where(entry => entry.Date.Date >= DateTime.Today)
                .OrderBy(entry => entry.Date)
                .Take(4)
                .ToList();

            _upcomingCount.Text = events.Count(entry =>
                (entry.Date.Kind == DateTimeKind.Utc
                    ? entry.Date.ToLocalTime()
                    : entry.Date).Date >= DateTime.Today).ToString();
            _memberCount.Text = members.Count.ToString();
            _inventoryCount.Text = inventory.Count.ToString();

            _eventList.Children.Clear();
            _eventList.Children.Add(_status);
            _eventList.Children.Add(_retryButton);
            if (upcomingEvents.Count == 0)
            {
                _status.Text = "Não existem eventos futuros na base de dados.";
            }
            else
            {
                _status.Text = "";
                foreach (var entry in upcomingEvents)
                {
                    _eventList.Children.Add(ViewTheme.EventCard(
                        entry.Event.Name,
                        $"{entry.Date:dd/MM/yyyy} · {entry.Event.Location}"));
                }
            }
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            _status.Text = $"Não foi possível carregar os dados: {exception.Message}";
            _status.TextColor = ViewTheme.Error;
            _retryButton.IsVisible = true;
        }
    }
}

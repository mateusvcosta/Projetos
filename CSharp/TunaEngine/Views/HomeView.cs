using Microsoft.Maui.Layouts;

namespace TunaEngine.Views;

public sealed class HomeView : ContentView
{
    public HomeView(bool isWideLayout)
    {
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
            ViewTheme.Metric("PRÓXIMOS EVENTOS", "4", 12, 28, 24, ViewTheme.Accent),
            ViewTheme.Metric("MEMBROS ATIVOS", "—", 12, 28, 24),
            ViewTheme.Metric(
                "ITENS EM INVENTÁRIO",
                AppSession.Tables.Sum(table => table.Items.Count).ToString(), 12, 28, 24),
            ViewTheme.Metric(
                "DATA SELECIONADA",
                AppSession.FormatShortDate(AppSession.SelectedDate), 12, 16, 24)
        };
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

        var eventList = new VerticalStackLayout { Spacing = 12, Margin = new Thickness(0, 11, 0, 0) };
        for (var index = 0; index < 4; index++)
            eventList.Children.Add(ViewTheme.EventCard("Atuação", "Conteúdo de demonstração"));

        var agenda = new VerticalStackLayout { Spacing = 0 };
        agenda.Children.Add(agendaHeading);
        agenda.Children.Add(eventList);

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
    }
}

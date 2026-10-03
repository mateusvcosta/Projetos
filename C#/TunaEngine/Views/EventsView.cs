namespace TunaEngine.Views;

public sealed class EventsView : ContentView
{
    private readonly VerticalStackLayout _agenda = new() { Spacing = 16 };

    public EventsView()
    {
        var content = new VerticalStackLayout { Spacing = 24 };
        var title = ViewTheme.TextLabel("Eventos", 30, true);
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        content.Children.Add(title);
        content.Children.Add(ViewTheme.TextLabel(
            "Datas e atividades da equipa.",
            14,
            color: ViewTheme.Muted));
        content.Children.Add(new SessionCalendarView(RefreshAgenda));
        content.Children.Add(ViewTheme.TextLabel("Agenda", 20, true));
        content.Children.Add(_agenda);
        content.Children.Add(ViewTheme.Action("+ Novo evento", AddEvent));
        Content = content;
        RefreshAgenda();
    }

    private void RefreshAgenda()
    {
        _agenda.Children.Clear();
        var events = AppSession.Events
            .Where(entry => entry.Date is null ||
                            entry.Date.Value.Date == AppSession.SelectedDate.Date)
            .ToList();

        if (events.Count == 0)
        {
            _agenda.Children.Add(ViewTheme.Card(ViewTheme.TextLabel(
                "Sem eventos para esta data.",
                color: ViewTheme.Muted)));
            return;
        }

        foreach (var entry in events)
        {
            var description = entry.Date.HasValue
                ? $"{entry.Date.Value:dd/MM/yyyy} · {entry.Description}"
                : entry.Description;
            _agenda.Children.Add(ViewTheme.EventCard(entry.Title, description));
        }
    }

    private async Task AddEvent()
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var title = await shell.DisplayPromptAsync(
            "Novo evento", "Título do evento:", "Continuar", "Cancelar");
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        var description = await shell.DisplayPromptAsync(
            "Descrição", "Detalhes do evento:", "Criar", "Cancelar");
        if (description is null)
        {
            return;
        }

        AppSession.Events.Add(new AgendaEvent
        {
            Title = title.Trim(),
            Description = description.Trim(),
            Date = AppSession.SelectedDate
        });
        RefreshAgenda();
    }
}

internal sealed class SessionCalendarView : ContentView
{
    private static readonly string[] WeekHeaders = ["SEG", "TER", "QUA", "QUI", "SEX", "SÁB", "DOM"];
    private DateTime _selected;
    private DateTime _month;
    private readonly Action _onSelected;
    private readonly bool _figmaSize;

    public SessionCalendarView(Action onSelected, bool figmaSize = false)
    {
        _selected = AppSession.SelectedDate.Date;
        _month = new DateTime(_selected.Year, _selected.Month, 1);
        _onSelected = onSelected;
        _figmaSize = figmaSize;
        Render();
    }

    private void Render()
    {
        var stack = new VerticalStackLayout { Spacing = 0 };
        var header = new Grid
        {
            Padding = new Thickness(20, 0),
            HeightRequest = 51,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(28)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(28))
            }
        };
        header.Add(CreateMonthButton("‹", -1), 0, 0);

        var monthTitle = AppSession.Portuguese.TextInfo.ToTitleCase(
            _month.ToString("MMMM yyyy", AppSession.Portuguese));
        var title = ViewTheme.TextLabel(monthTitle, 15, true);
        title.HorizontalTextAlignment = TextAlignment.Center;
        header.Add(title, 1, 0);
        header.Add(CreateMonthButton("›", 1), 2, 0);
        stack.Children.Add(header);
        stack.Children.Add(ViewTheme.Divider());

        var calendar = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            Margin = new Thickness(12, 0)
        };
        for (var day = 0; day < 7; day++)
        {
            calendar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        calendar.RowDefinitions.Add(new RowDefinition(new GridLength(36)));
        for (var column = 0; column < WeekHeaders.Length; column++)
        {
            var label = ViewTheme.TextLabel(
                WeekHeaders[column],
                11,
                true,
                column == 4 ? ViewTheme.Accent : ViewTheme.Muted);
            label.HorizontalTextAlignment = TextAlignment.Center;
            calendar.Add(label, column, 0);
        }

        var offset = ((int)_month.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(_month.Year, _month.Month);
        var weekCount = (offset + daysInMonth + 6) / 7;
        var dayRowHeight = weekCount == 6 ? 32 : 40;
        for (var week = 0; week < weekCount; week++)
        {
            calendar.RowDefinitions.Add(new RowDefinition(new GridLength(dayRowHeight)));
        }

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(_month.Year, _month.Month, day);
            var index = offset + day - 1;
            var label = ViewTheme.TextLabel(
                day.ToString(),
                13,
                date.Date == _selected.Date,
                date.Date == _selected.Date
                    ? ViewTheme.Text
                    : Color.FromArgb("#CCCCDD"));
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.VerticalTextAlignment = TextAlignment.Center;

            var cell = new Border
            {
                BackgroundColor = date.Date == _selected.Date
                    ? ViewTheme.Accent
                    : Colors.Transparent,
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                WidthRequest = 30,
                HeightRequest = 30,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = label
            };
            SemanticProperties.SetDescription(cell,
                date.ToString("D", AppSession.Portuguese));
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                _selected = date;
                AppSession.SelectedDate = date;
                Render();
                _onSelected();
            };
            cell.GestureRecognizers.Add(tap);
            calendar.Add(cell, index % 7, index / 7 + 1);
        }

        stack.Children.Add(calendar);
        var footer = new Grid
        {
            HeightRequest = 34,
            Padding = new Thickness(12, 0),
            RowDefinitions = { new RowDefinition(new GridLength(1)), new RowDefinition(GridLength.Star) }
        };
        footer.Add(ViewTheme.Divider(), 0, 0);
        var selectedLabel = ViewTheme.TextLabel(
            AppSession.FormatLongDate(_selected),
            11,
            color: ViewTheme.Muted);
        footer.Add(selectedLabel, 0, 1);
        stack.Children.Add(footer);
        var calendarCard = ViewTheme.Card(stack, 0);
        if (_figmaSize)
        {
            calendarCard.HeightRequest = 324;
            calendarCard.HorizontalOptions = LayoutOptions.Fill;
        }

        Content = calendarCard;
    }

    private Button CreateMonthButton(string text, int offset)
    {
        var button = new Button
        {
            Text = text,
            FontSize = 14,
            CornerRadius = 6,
            Padding = 0,
            WidthRequest = 28,
            HeightRequest = 28,
            MinimumHeightRequest = 0,
            MinimumWidthRequest = 0,
            TextColor = ViewTheme.Text,
            BackgroundColor = ViewTheme.Stroke,
            BorderWidth = 0
        };
        button.Clicked += (_, _) =>
        {
            _month = _month.AddMonths(offset);
            Render();
        };
        return button;
    }
}

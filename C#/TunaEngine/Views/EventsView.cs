using System.Globalization;

namespace TunaEngine.Views;

public sealed class EventsView : RecordSectionView
{
    private static readonly CultureInfo PortugueseCulture = CultureInfo.GetCultureInfo("pt-PT");
    private readonly Label monthLabel = new();
    private readonly Label selectedDateLabel = new();
    private readonly Grid dateGrid = CreateDateGrid();
    private DateTime displayedMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateTime selectedDate = DateTime.Today;

    public EventsView(bool isWideLayout)
        : base("Eventos", "Datas e atividades da equipa.", "EVENTOS", ["Reunião", "Entrega", "Evento"], isWideLayout)
    {
        var content = (VerticalStackLayout)Content;
        content.Children.Insert(2, CreateCalendar());
        RefreshCalendar();
    }

    private Border CreateCalendar()
    {
        var previousMonthButton = CreateMonthButton("‹", "Mês anterior", "PreviousMonth");
        previousMonthButton.Clicked += (_, _) => ChangeMonth(-1);
        var nextMonthButton = CreateMonthButton("›", "Mês seguinte", "NextMonth");
        nextMonthButton.Clicked += (_, _) => ChangeMonth(1);

        monthLabel.FontSize = 18;
        monthLabel.FontAttributes = FontAttributes.Bold;
        monthLabel.TextColor = Color.FromArgb("#F3F5F4");
        monthLabel.HorizontalTextAlignment = TextAlignment.Center;
        monthLabel.VerticalTextAlignment = TextAlignment.Center;
        monthLabel.HorizontalOptions = LayoutOptions.Fill;

        var monthHeader = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            VerticalOptions = LayoutOptions.Center
        };
        monthHeader.Add(previousMonthButton, 0, 0);
        monthHeader.Add(monthLabel, 1, 0);
        monthHeader.Add(nextMonthButton, 2, 0);

        var weekdayHeader = new Grid
        {
            ColumnDefinitions = CreateWeekColumns(),
            ColumnSpacing = 0,
            Margin = new Thickness(0, 12, 0, 4)
        };
        string[] weekdays = ["SEG", "TER", "QUA", "QUI", "SEX", "SÁB", "DOM"];
        for (var index = 0; index < weekdays.Length; index++)
        {
            weekdayHeader.Add(new Label
            {
                Text = weekdays[index],
                FontSize = 10,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#91A0A5"),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }, index, 0);
        }

        var calendarContent = new VerticalStackLayout
        {
            Spacing = 0,
            Padding = new Thickness(14),
            Children =
            {
                monthHeader,
                weekdayHeader,
                dateGrid,
                selectedDateLabel
            }
        };

        selectedDateLabel.FontSize = 13;
        selectedDateLabel.TextColor = Color.FromArgb("#AAB7BA");
        selectedDateLabel.Margin = new Thickness(4, 12, 4, 0);

        return new Border
        {
            BackgroundColor = Color.FromArgb("#1A262E"),
            Stroke = Color.FromArgb("#2D3A42"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = calendarContent
        };
    }

    private static Button CreateMonthButton(string text, string description, string automationId)
    {
        var button = new Button
        {
            Text = text,
            FontSize = 22,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#202D35"),
            BorderColor = Color.FromArgb("#2D3A42"),
            BorderWidth = 1,
            Padding = 0,
            WidthRequest = 40,
            HeightRequest = 40,
            MinimumWidthRequest = 40,
            MinimumHeightRequest = 40,
            CornerRadius = 6,
            HorizontalOptions = LayoutOptions.Fill,
            AutomationId = automationId
        };
        var pointer = new PointerGestureRecognizer();
        pointer.PointerEntered += (_, _) => button.TextColor = Color.FromArgb("#D94F65");
        pointer.PointerExited += (_, _) => button.TextColor = Colors.White;
        button.GestureRecognizers.Add(pointer);
        SemanticProperties.SetDescription(button, description);
        return button;
    }

    private void ChangeMonth(int offset)
    {
        displayedMonth = displayedMonth.AddMonths(offset);
        var day = Math.Min(selectedDate.Day, DateTime.DaysInMonth(displayedMonth.Year, displayedMonth.Month));
        selectedDate = new DateTime(displayedMonth.Year, displayedMonth.Month, day);
        RefreshCalendar();
    }

    private void RefreshCalendar()
    {
        monthLabel.Text = PortugueseCulture.TextInfo.ToTitleCase(displayedMonth.ToString("MMMM yyyy", PortugueseCulture));
        selectedDateLabel.Text = $"Selecionado: {selectedDate.ToString("dddd, d 'de' MMMM 'de' yyyy", PortugueseCulture)}";
        dateGrid.Children.Clear();

        var firstDay = new DateTime(displayedMonth.Year, displayedMonth.Month, 1);
        var mondayOffset = ((int)firstDay.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(displayedMonth.Year, displayedMonth.Month);

        for (var slot = 0; slot < 42; slot++)
        {
            var dayNumber = slot - mondayOffset + 1;
            if (dayNumber < 1 || dayNumber > daysInMonth)
            {
                continue;
            }

            var date = new DateTime(displayedMonth.Year, displayedMonth.Month, dayNumber);
            var isSelected = date.Date == selectedDate.Date;
            var isToday = date.Date == DateTime.Today;
            var dayButton = new Button
            {
                Text = dayNumber.ToString(),
                FontSize = 14,
                TextColor = isSelected ? Colors.White : Color.FromArgb("#F3F5F4"),
                BackgroundColor = isSelected ? Color.FromArgb("#D94F65") : Colors.Transparent,
                BorderColor = isToday && !isSelected ? Color.FromArgb("#D94F65") : Colors.Transparent,
                BorderWidth = isToday && !isSelected ? 1 : 0,
                CornerRadius = 6,
                Padding = 0,
                MinimumWidthRequest = 0,
                MinimumHeightRequest = 0,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };
            SemanticProperties.SetDescription(dayButton, date.ToString("D", PortugueseCulture));
            dayButton.Clicked += (_, _) =>
            {
                selectedDate = date;
                RefreshCalendar();
            };
            dateGrid.Add(dayButton, slot % 7, slot / 7);
        }
    }

    private static Grid CreateDateGrid()
    {
        var grid = new Grid
        {
            ColumnDefinitions = CreateWeekColumns(),
            ColumnSpacing = 0,
            RowSpacing = 2
        };
        for (var week = 0; week < 6; week++)
        {
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(42)));
        }

        return grid;
    }

    private static ColumnDefinitionCollection CreateWeekColumns()
    {
        var columns = new ColumnDefinitionCollection();
        for (var day = 0; day < 7; day++)
        {
            columns.Add(new ColumnDefinition(GridLength.Star));
        }

        return columns;
    }
}
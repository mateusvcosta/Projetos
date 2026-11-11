using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Models;
using TunaEngine.Services;
using TunaEngine.State;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class AddEventPopup : ContentView
{
    private readonly TaskCompletionSource<EventDraft?> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Entry _titleEntry;
    private readonly Entry _locationEntry;
    private readonly DatePicker _datePicker;
    private readonly Picker _memberPicker;
    private readonly Entry _attendanceEntry;
    private readonly Picker _instrumentPicker;
    private readonly Entry _quantityEntry;
    private readonly Picker _driverPicker;
    private readonly Entry _pickupEntry;
    private readonly Entry _departureEntry;
    private readonly Entry _seatsEntry;
    private readonly Entry _passengersEntry;
    private readonly VerticalStackLayout _participantsList = new() { Spacing = 4 };
    private readonly VerticalStackLayout _instrumentsList = new() { Spacing = 4 };
    private readonly VerticalStackLayout _ridesList = new() { Spacing = 4 };
    private readonly Label _errorLabel;
    private readonly List<ApiEventParticipant> _participants = [];
    private readonly List<ApiEventInstrument> _instruments = [];
    private readonly List<ApiEventRide> _rides = [];

    public Task<EventDraft?> Completion => _completion.Task;

    public AddEventPopup(ApiEvent? existing = null)
    {
        BackgroundColor = Colors.Transparent;
        Padding = 0;
        _titleEntry = CreateEntry("Nome do evento", 100, existing?.Name);
        _locationEntry = CreateEntry("Localização", 250, existing?.Location);
        _datePicker = new DatePicker
        {
            Date = existing?.Date.ToLocalTime().Date ?? AppSession.SelectedDate.Date,
            TextColor = ViewTheme.Text,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        _memberPicker = CreatePicker("Membro");
        _attendanceEntry = CreateEntry("Estado (presente, pendente...)", 40, "pendente");
        _instrumentPicker = CreatePicker("Instrumento do inventário");
        _quantityEntry = CreateEntry("Quantidade", 6, "1", Keyboard.Numeric);
        _driverPicker = CreatePicker("Condutor");
        _pickupEntry = CreateEntry("Local de recolha", 120);
        _departureEntry = CreateEntry("Hora de partida", 20);
        _seatsEntry = CreateEntry("Lugares", 4, "1", Keyboard.Numeric);
        _passengersEntry = CreateEntry("IDs dos passageiros separados por vírgula", 300);
        _errorLabel = new Label { IsVisible = false, FontSize = 13, TextColor = ViewTheme.Error };

        ConfigurePickers();
        if (existing is not null)
        {
            _participants.AddRange(existing.Participants);
            _instruments.AddRange(existing.Instruments_Needed);
            _rides.AddRange(existing.Boleias);
        }

        var cancel = CreateButton("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancel.Clicked += (_, _) => Cancel();
        var save = CreateButton(existing is null ? "Criar evento" : "Guardar alterações", Colors.White, ViewTheme.Accent);
        save.Clicked += (_, _) => Save();

        var buttons = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children = { cancel, save }
        };
        Grid.SetColumn(save, 1);

        var form = new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                ViewTheme.TextLabel(existing is null ? "Novo evento" : "Editar evento", 22, true),
                ViewTheme.TextLabel("Dados gerais", 14, true, ViewTheme.Muted),
                _titleEntry, _locationEntry, _datePicker
            }
        };

        if (existing is not null)
        {
            form.Children.Add(BuildParticipantsSection());
            form.Children.Add(BuildInstrumentsSection());
            form.Children.Add(BuildRidesSection());
        }

        form.Children.Add(_errorLabel);
        form.Children.Add(buttons);

        Content = new Border
        {
            WidthRequest = 560,
            MaximumWidthRequest = 720,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new ScrollView { MaximumHeightRequest = 760, Content = form }
        };
        RefreshLists();
    }

    public AddEventPopup() : this(null) { }

    public AddEventPopup(string title, string location, DateTime date)
        : this(new ApiEvent { Name = title, Location = location, Date = date })
    {
    }

    private void ConfigurePickers()
    {
        _memberPicker.ItemsSource = AppSession.Members
            .Select(member => new PickerOption(member.Id, member.Name))
            .ToList();
        _instrumentPicker.ItemsSource = AppSession.Tables
            .SelectMany(table => table.Items)
            .Select(item => new PickerOption(item.Id, item.Name))
            .ToList();
        _driverPicker.ItemsSource = AppSession.Members
            .Select(member => new PickerOption(member.Id, member.Name))
            .ToList();
    }

    private View BuildParticipantsSection()
    {
        var add = CreateButton("Adicionar participante", Colors.White, ViewTheme.SurfaceDark);
        add.Clicked += (_, _) =>
        {
            if (_memberPicker.SelectedItem is not PickerOption member ||
                string.IsNullOrWhiteSpace(_attendanceEntry.Text))
            {
                ShowValidation("Selecione um membro e indique o estado da presença.");
                return;
            }

            _participants.Add(new ApiEventParticipant
            {
                MemberId = member.Id,
                AttendanceStatus = _attendanceEntry.Text.Trim()
            });
            _memberPicker.SelectedItem = null;
            RefreshLists();
        };
        return Section("Participantes", new VerticalStackLayout
        {
            Spacing = 6,
            Children = { new HorizontalStackLayout
            {
                Spacing = 6,
                Children = { _memberPicker, _attendanceEntry, add }
            }, _participantsList }
        });
    }

    private View BuildInstrumentsSection()
    {
        var add = CreateButton("Adicionar instrumento", Colors.White, ViewTheme.SurfaceDark);
        add.Clicked += (_, _) =>
        {
            if (_instrumentPicker.SelectedItem is not PickerOption instrument ||
                !int.TryParse(_quantityEntry.Text, out var quantity) || quantity <= 0)
            {
                ShowValidation("Selecione um instrumento e indique uma quantidade positiva.");
                return;
            }

            _instruments.Add(new ApiEventInstrument
            {
                InventoryId = instrument.Id,
                Designation = instrument.Name,
                Quantity = quantity
            });
            _instrumentPicker.SelectedItem = null;
            RefreshLists();
        };
        return Section("Instrumentos necessários", new VerticalStackLayout
        {
            Spacing = 6,
            Children = { new HorizontalStackLayout
            {
                Spacing = 6,
                Children = { _instrumentPicker, _quantityEntry, add }
            }, _instrumentsList }
        });
    }

    private View BuildRidesSection()
    {
        var add = CreateButton("Adicionar boleia", Colors.White, ViewTheme.SurfaceDark);
        add.Clicked += (_, _) =>
        {
            if (_driverPicker.SelectedItem is not PickerOption driver ||
                !int.TryParse(_seatsEntry.Text, out var seats) || seats < 0 ||
                string.IsNullOrWhiteSpace(_pickupEntry.Text))
            {
                ShowValidation("Selecione o condutor, local e lugares válidos.");
                return;
            }

            var passengers = (_passengersEntry.Text ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
            _rides.Add(new ApiEventRide
            {
                DriverId = driver.Id,
                PickupLocation = _pickupEntry.Text.Trim(),
                DepartureHour = _departureEntry.Text?.Trim() ?? "",
                AvailableSeats = seats,
                PassengerIds = passengers
            });
            _driverPicker.SelectedItem = null;
            RefreshLists();
        };
        return Section("Boleias", new VerticalStackLayout
        {
            Spacing = 6,
            Children =
            {
                new HorizontalStackLayout { Spacing = 6, Children = { _driverPicker, _pickupEntry } },
                new HorizontalStackLayout { Spacing = 6, Children = { _departureEntry, _seatsEntry, _passengersEntry, add } },
                _ridesList
            }
        });
    }

    private void RefreshLists()
    {
        RefreshList(_participantsList, _participants.Select(item => $"Membro: {FindName(item.MemberId)} · {item.AttendanceStatus}").ToList(),
            index => _participants.RemoveAt(index));
        RefreshList(_instrumentsList, _instruments.Select(item => $"{item.Designation} · quantidade: {item.Quantity}").ToList(),
            index => _instruments.RemoveAt(index));
        RefreshList(_ridesList, _rides.Select(item =>
            $"Condutor: {FindName(item.DriverId)} · {item.PickupLocation} · {item.AvailableSeats} lugares · {item.PassengerIds.Count} passageiros").ToList(),
            index => _rides.RemoveAt(index));
    }

    private static void RefreshList(VerticalStackLayout target, List<string> values, Action<int> remove)
    {
        target.Children.Clear();
        for (var index = 0; index < values.Count; index++)
        {
            var current = index;
            var row = new HorizontalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    ViewTheme.TextLabel(values[index], 13, color: ViewTheme.Muted),
                    ViewTheme.Action("Remover", () =>
                    {
                        remove(current);
                        return Task.CompletedTask;
                    }, primary: false)
                }
            };
            target.Children.Add(row);
        }
    }

    private string FindName(string id) =>
        AppSession.Members.FirstOrDefault(member => member.Id == id)?.Name ?? id;

    private static Border Section(string title, View content) =>
        new()
        {
            Padding = 12,
            BackgroundColor = ViewTheme.SurfaceDark,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = { ViewTheme.TextLabel(title, 15, true), content }
            }
        };

    private static Entry CreateEntry(string placeholder, int maxLength, string? value = null, Keyboard? keyboard = null) =>
        new()
        {
            Placeholder = placeholder,
            Text = value,
            MaxLength = maxLength,
            Keyboard = keyboard ?? Keyboard.Default,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark,
            HorizontalOptions = LayoutOptions.Fill
        };

    private static Picker CreatePicker(string title) => new()
    {
        Title = title,
        TextColor = ViewTheme.Text,
        TitleColor = ViewTheme.Muted,
        BackgroundColor = ViewTheme.SurfaceDark,
        HorizontalOptions = LayoutOptions.Fill,
        ItemDisplayBinding = new Binding(nameof(PickerOption.Name))
    };

    private static StateButton CreateButton(string text, Color textColor, Color backgroundColor) => new()
    {
        Text = text,
        TextColor = textColor,
        BackgroundColor = backgroundColor,
        CornerRadius = 10,
        Padding = new Thickness(10, 6)
    };

    private void Save()
    {
        var title = _titleEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(_locationEntry.Text))
        {
            ShowValidation("Introduza o nome e a localização do evento.");
            return;
        }

        _completion.TrySetResult(new EventDraft(
            title,
            _locationEntry.Text.Trim(),
            new DateTimeOffset(_datePicker.Date.GetValueOrDefault()),
            [.. _participants],
            [.. _instruments],
            [.. _rides]));
    }

    private void ShowValidation(string message)
    {
        _errorLabel.Text = message;
        _errorLabel.IsVisible = true;
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(null));
}

internal sealed record PickerOption(string Id, string Name);

public sealed record EventDraft(
    string Title,
    string Description,
    DateTimeOffset Date,
    List<ApiEventParticipant> Participants,
    List<ApiEventInstrument> Instruments,
    List<ApiEventRide> Rides);

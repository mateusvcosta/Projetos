using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Services;
using TunaEngine.State;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class EventDetailsPopup : ContentView
{
    private readonly TaskCompletionSource<bool> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly StateButton _participationButton;
    private readonly Func<Task<bool>> _toggleParticipation;
    private readonly Picker _ridePicker;
    private readonly Entry _pickupEntry;
    private readonly Entry _seatsEntry;
    private readonly Label _transportError;
    private readonly ApiEvent _event;
    private readonly Func<TransportChoice, Task<bool>> _updateTransport;

    public Task<bool> Completion => _completion.Task;

    public EventDetailsPopup(
        ApiEvent item,
        Func<Task<bool>> toggleParticipation,
        Func<TransportChoice, Task<bool>> updateTransport)
    {
        _event = item;
        _toggleParticipation = toggleParticipation;
        _updateTransport = updateTransport;
        var close = new StateButton
        {
            Text = "Fechar",
            TextColor = ViewTheme.Text,
            BackgroundColor = ViewTheme.SurfaceDark,
            CornerRadius = 10
        };
        close.Clicked += (_, _) => Cancel();
        var participating = IsParticipating(item);
        _participationButton = new StateButton
        {
            Text = participating ? "Não participar" : "Participar",
            TextColor = Colors.White,
            BackgroundColor = participating ? ViewTheme.Danger : ViewTheme.Accent,
            CornerRadius = 10
        };
        ViewTheme.BindAction(_participationButton, ToggleParticipation);
        _ridePicker = new Picker
        {
            Title = "Escolher boleia",
            TextColor = ViewTheme.Text,
            TitleColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark,
            ItemDisplayBinding = new Binding(nameof(RideOption.Description))
        };
        _pickupEntry = new Entry
        {
            Placeholder = "Local de recolha",
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        _seatsEntry = new Entry
        {
            Placeholder = "Lugares disponíveis",
            Text = "1",
            Keyboard = Keyboard.Numeric,
            TextColor = ViewTheme.Text,
            PlaceholderColor = ViewTheme.Muted,
            BackgroundColor = ViewTheme.SurfaceDark
        };
        _transportError = new Label
        {
            IsVisible = false,
            TextColor = ViewTheme.Error,
            FontSize = 13
        };

        var content = new VerticalStackLayout { Spacing = 12 };
        content.Children.Add(ViewTheme.TextLabel(item.Name, 23, true));
        content.Children.Add(ViewTheme.TextLabel(
            $"{item.Date.ToLocalTime():dd/MM/yyyy HH:mm} · {item.Location}",
            14,
            color: ViewTheme.Muted));
        content.Children.Add(Section("Participantes", item.Participants.Count == 0
            ? ["Nenhum participante registado."]
            : item.Participants.Select(participant =>
                $"{FindMember(participant.MemberId)} · {participant.AttendanceStatus}").ToList()));
        content.Children.Add(Section("Instrumentos", item.Instruments_Needed.Count == 0
            ? ["Nenhum instrumento registado."]
            : item.Instruments_Needed.Select(instrument =>
                $"{instrument.Designation} · quantidade: {instrument.Quantity}").ToList()));
        content.Children.Add(Section("Boleias", item.Boleias.Count == 0
            ? ["Nenhuma boleia registada."]
            : item.Boleias.Select(ride =>
                $"{FindMember(ride.DriverId)} · {ride.PickupLocation} · " +
                $"{ride.AvailableSeats} lugares · {ride.PassengerIds.Count} passageiros").ToList()));
        content.Children.Add(_participationButton);
        content.Children.Add(BuildTransportSection());
        content.Children.Add(close);

        Content = new Border
        {
            WidthRequest = 500,
            MaximumWidthRequest = 680,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new ScrollView { MaximumHeightRequest = 700, Content = content }
        };
    }

    private View BuildTransportSection()
    {
        RefreshRidePicker();
        var drive = new StateButton
        {
            Text = "Oferecer boleia",
            TextColor = Colors.White,
            BackgroundColor = ViewTheme.SurfaceDark,
            CornerRadius = 10
        };
        drive.Clicked += async (_, _) => await OfferRide();

        var passenger = new StateButton
        {
            Text = "Preciso de boleia",
            TextColor = Colors.White,
            BackgroundColor = ViewTheme.SurfaceDark,
            CornerRadius = 10
        };
        passenger.Clicked += async (_, _) => await JoinRide();

        return Section("Transporte", new List<string>
        {
            "Ofereça lugares no seu carro ou escolha uma boleia disponível."
        }, new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                _pickupEntry,
                _seatsEntry,
                drive,
                _ridePicker,
                passenger,
                _transportError
            }
        });
    }

    private async Task OfferRide()
    {
        if (string.IsNullOrWhiteSpace(AppSession.ProfileMemberId) ||
            string.IsNullOrWhiteSpace(_pickupEntry.Text) ||
            !int.TryParse(_seatsEntry.Text, out var seats) || seats <= 0)
        {
            ShowTransportError("Indique o local de recolha e um número de lugares válido.");
            return;
        }

        if (await _updateTransport(new TransportChoice(
                TransportAction.OfferRide,
                _pickupEntry.Text.Trim(),
                seats,
                null)))
        {
            _pickupEntry.Text = "";
            RefreshRidePicker();
        }
    }

    private async Task JoinRide()
    {
        if (_ridePicker.SelectedItem is not RideOption ride)
        {
            ShowTransportError("Escolha uma boleia disponível.");
            return;
        }

        if (await _updateTransport(new TransportChoice(
                TransportAction.JoinRide,
                "",
                0,
                ride.Index)))
        {
            RefreshRidePicker();
        }
    }

    private void RefreshRidePicker()
    {
        _ridePicker.ItemsSource = _event.Boleias
            .Select((ride, index) => new RideOption(
                index,
                $"{ride.PickupLocation} · {ride.AvailableSeats - ride.PassengerIds.Count} lugares livres"))
            .Where(ride => ride.Description.EndsWith(" lugares livres", StringComparison.Ordinal)
                && _event.Boleias[ride.Index].AvailableSeats >
                   _event.Boleias[ride.Index].PassengerIds.Count)
            .ToList();
        _ridePicker.SelectedItem = null;
    }

    private void ShowTransportError(string message)
    {
        _transportError.Text = message;
        _transportError.IsVisible = true;
    }

    private async Task ToggleParticipation()
    {
        var participating = await _toggleParticipation();
        _participationButton.Text = participating ? "Não participar" : "Participar";
        _participationButton.BackgroundColor = participating ? ViewTheme.Danger : ViewTheme.Accent;
    }

    private static bool IsParticipating(ApiEvent item) =>
        !string.IsNullOrWhiteSpace(AppSession.ProfileMemberId) &&
        item.Participants.Any(participant =>
            string.Equals(
                participant.MemberId,
                AppSession.ProfileMemberId,
                StringComparison.OrdinalIgnoreCase));

    private static Border Section(string title, List<string> values, View? extra = null)
    {
        var stack = new VerticalStackLayout
        {
            Spacing = 5
        };
        stack.Children.Add(ViewTheme.TextLabel(title, 15, true));
        foreach (var value in values)
        {
            stack.Children.Add(ViewTheme.TextLabel(value, 13, color: ViewTheme.Muted));
        }
        if (extra is not null)
        {
            stack.Children.Add(extra);
        }

        return new Border
        {
            Padding = 12,
            BackgroundColor = ViewTheme.SurfaceDark,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Content = stack
        };
    }

    private static string FindMember(string id) =>
        AppSession.Members.FirstOrDefault(member => member.Id == id)?.Name ?? id;

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(true));
}

public enum TransportAction
{
    OfferRide,
    JoinRide
}

public sealed record TransportChoice(
    TransportAction Action,
    string PickupLocation,
    int AvailableSeats,
    int? RideIndex);

internal sealed record RideOption(int Index, string Description);

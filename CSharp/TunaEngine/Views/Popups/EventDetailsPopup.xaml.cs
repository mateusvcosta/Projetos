using Microsoft.Maui.Controls.Shapes;
using TunaEngine.Services;
using TunaEngine.State;
using TunaEngine.Views;

namespace TunaEngine.Views.Popups;

public sealed class EventDetailsPopup : ContentView
{
    private readonly TaskCompletionSource<bool> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task<bool> Completion => _completion.Task;

    public EventDetailsPopup(ApiEvent item)
    {
        var close = new StateButton
        {
            Text = "Fechar",
            TextColor = ViewTheme.Text,
            BackgroundColor = ViewTheme.SurfaceDark,
            CornerRadius = 10
        };
        close.Clicked += (_, _) => Cancel();

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

    private static Border Section(string title, List<string> values)
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

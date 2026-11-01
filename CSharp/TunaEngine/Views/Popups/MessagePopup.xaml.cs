using Microsoft.Maui.Controls.Shapes;

namespace TunaEngine.Views.Popups;

public sealed class MessagePopup : ContentView
{
    private readonly TaskCompletionSource<bool> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task<bool> Completion => _completion.Task;

    public MessagePopup(string title, string message)
    {
        var close = new StateButton
        {
            Text = "OK",
            TextColor = Colors.White,
            BackgroundColor = ViewTheme.Accent,
            CornerRadius = 10
        };
        close.Clicked += (_, _) => _completion.TrySetResult(true);

        Content = new Border
        {
            WidthRequest = 360,
            MaximumWidthRequest = 480,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Error,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    ViewTheme.TextLabel(title, 22, true, ViewTheme.Error),
                    ViewTheme.TextLabel(message, 14, color: ViewTheme.Muted),
                    close
                }
            }
        };
    }

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(true));
}

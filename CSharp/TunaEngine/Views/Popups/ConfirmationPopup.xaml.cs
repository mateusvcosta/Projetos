using Microsoft.Maui.Controls.Shapes;

namespace TunaEngine.Views.Popups;

public sealed class ConfirmationPopup : ContentView
{
    private readonly TaskCompletionSource<bool> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task<bool> Completion => _completion.Task;

    public ConfirmationPopup(string title, string message, string confirmText = "Confirmar")
    {
        var cancel = Button("Cancelar", ViewTheme.Text, ViewTheme.SurfaceDark);
        cancel.Clicked += (_, _) => Cancel();
        var confirm = Button(confirmText, Colors.White, ViewTheme.Danger);
        confirm.Clicked += (_, _) => _completion.TrySetResult(true);
        var buttons = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children = { cancel, confirm }
        };
        Grid.SetColumn(confirm, 1);

        Content = new Border
        {
            WidthRequest = 360,
            MaximumWidthRequest = 480,
            Padding = 22,
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    ViewTheme.TextLabel(title, 22, true),
                    ViewTheme.TextLabel(message, 14, color: ViewTheme.Muted),
                    buttons
                }
            }
        };
    }

    private static StateButton Button(string text, Color textColor, Color background) => new()
    {
        Text = text,
        TextColor = textColor,
        BackgroundColor = background,
        CornerRadius = 10
    };

    public void Cancel() =>
        MainThread.BeginInvokeOnMainThread(() => _completion.TrySetResult(false));
}

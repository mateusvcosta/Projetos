namespace TunaEngine.Views;

internal static class ModalOverlay
{
    public static async Task<T?> ShowAsync<T>(
        Page page,
        View content,
        Task<T?> completion,
        Action cancel)
    {
        if (page is not ContentPage contentPage)
        {
            throw new InvalidOperationException("A modal form requires a content page.");
        }

        var pageContent = contentPage.Content;
        if (pageContent is null)
        {
            throw new InvalidOperationException("The page has no content to display behind the modal.");
        }

        var root = new Grid();
        var overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#99000000")
        };
        var backdrop = new BoxView
        {
            Color = Colors.Transparent
        };
        var dismissGesture = new TapGestureRecognizer();
        dismissGesture.Tapped += (_, _) => cancel();
        backdrop.GestureRecognizers.Add(dismissGesture);

        content.HorizontalOptions = LayoutOptions.Center;
        content.VerticalOptions = LayoutOptions.Center;
        overlay.Children.Add(backdrop);
        overlay.Children.Add(content);

        contentPage.Content = null;
        root.Children.Add(pageContent);
        root.Children.Add(overlay);
        contentPage.Content = root;

        try
        {
            return await completion;
        }
        finally
        {
            if (ReferenceEquals(contentPage.Content, root))
            {
                root.Children.Remove(pageContent);
                overlay.Children.Remove(content);
                contentPage.Content = pageContent;
            }

            root.Children.Clear();
            overlay.Children.Clear();
        }
    }
}

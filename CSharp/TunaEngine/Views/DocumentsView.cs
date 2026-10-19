using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;

namespace TunaEngine.Views;

public sealed class DocumentsView : ContentView
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Label _status = ViewTheme.TextLabel("A carregar documentos...", 13, color: ViewTheme.Muted);
    private readonly Button _retryButton;
    private readonly VerticalStackLayout _documents = new() { Spacing = 12 };

    public DocumentsView(TunaEngineApiClient apiClient)
    {
        _apiClient = apiClient;
        _retryButton = ViewTheme.Action("Tentar novamente", LoadDocumentsAsync, primary: false);
        _retryButton.IsVisible = false;

        var content = new VerticalStackLayout { Spacing = 16 };
        var title = ViewTheme.TextLabel("Documentos", 30, true);
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        content.Children.Add(title);
        content.Children.Add(ViewTheme.TextLabel(
            "Documentos guardados na base de dados.",
            14,
            color: ViewTheme.Muted));
        content.Children.Add(_status);
        content.Children.Add(_retryButton);
        content.Children.Add(_documents);
        Content = content;

        _ = LoadDocumentsAsync();
    }

    private async Task LoadDocumentsAsync()
    {
        _status.Text = "A carregar documentos...";
        _status.TextColor = ViewTheme.Muted;
        _retryButton.IsVisible = false;

        try
        {
            var documents = await _apiClient.GetAsync<List<ApiDocument>>("api/documents");
            _documents.Children.Clear();

            if (documents.Count == 0)
            {
                _status.Text = "Não existem documentos na base de dados.";
                return;
            }

            _status.Text = "";
            foreach (var document in documents)
            {
                var details = new VerticalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        ViewTheme.TextLabel(document.Type, 16, true),
                        ViewTheme.TextLabel(document.FilePath, 13, color: ViewTheme.Muted),
                        ViewTheme.TextLabel(
                            $"{document.DateCreated.ToLocalTime():dd/MM/yyyy} · {document.AddedBy}",
                            12,
                            color: ViewTheme.Muted)
                    }
                };
                if (!string.IsNullOrWhiteSpace(document.Notes))
                {
                    details.Children.Add(ViewTheme.TextLabel(
                        document.Notes,
                        13,
                        color: ViewTheme.Muted));
                }

                _documents.Children.Add(ViewTheme.Card(details));
            }
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            _status.Text = $"Não foi possível carregar os documentos: {exception.Message}";
            _status.TextColor = ViewTheme.Error;
            _retryButton.IsVisible = true;
        }
    }
}

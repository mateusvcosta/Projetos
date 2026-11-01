using System.Net.Http.Json;
using System.Text.Json;

namespace TunaEngine.Services;

public sealed class TunaEngineApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<T> GetAsync<T>(
        string relativeUri,
        CancellationToken cancellationToken = default)
        where T : class =>
        await httpClient.GetFromJsonAsync<T>(
            ResolveUri(relativeUri), JsonOptions, cancellationToken)
        ?? throw new InvalidOperationException(
            $"The API returned an empty response for '{relativeUri}'.");

    public async Task<T> PostAsync<T>(
        string relativeUri,
        object request,
        CancellationToken cancellationToken = default)
        where T : class
    {
        using var response = await httpClient.PostAsJsonAsync(
            ResolveUri(relativeUri), request, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, relativeUri, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException(
                $"The API returned an empty response for '{relativeUri}'.");
    }

    public async Task PutAsync(
        string relativeUri,
        object request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            ResolveUri(relativeUri), request, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, relativeUri, cancellationToken);
    }

    public async Task DeleteAsync(
        string relativeUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(
            ResolveUri(relativeUri), cancellationToken);
        await EnsureSuccessAsync(response, relativeUri, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string relativeUri,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(
            $"The API returned {(int)response.StatusCode} for '{relativeUri}': {details}");
    }

    private static Uri ResolveUri(string relativeUri)
    {
        var serviceUrl = relativeUri switch
        {
            var uri when uri.StartsWith("api/members", StringComparison.OrdinalIgnoreCase)
                || uri.StartsWith("api/roles", StringComparison.OrdinalIgnoreCase)
                => ApiServiceUrls.Members,
            var uri when uri.StartsWith("api/events", StringComparison.OrdinalIgnoreCase)
                => ApiServiceUrls.Events,
            var uri when uri.StartsWith("api/documents", StringComparison.OrdinalIgnoreCase)
                || uri.StartsWith("api/doc-types", StringComparison.OrdinalIgnoreCase)
                => ApiServiceUrls.Documents,
            var uri when uri.StartsWith("api/inventory", StringComparison.OrdinalIgnoreCase)
                => ApiServiceUrls.Inventory,
            _ => throw new ArgumentException(
                $"No microservice is configured for URI '{relativeUri}'.",
                nameof(relativeUri))
        };

        return new Uri(new Uri(serviceUrl), relativeUri);
    }
}

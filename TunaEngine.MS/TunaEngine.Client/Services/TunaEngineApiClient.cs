using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TunaEngine.Services;

public sealed record ApiServiceEndpoints(
    Uri Auth,
    Uri Members,
    Uri Events,
    Uri Inventory)
{
    public Uri Resolve(string relativeUri)
    {
        var root = relativeUri switch
        {
            var path when path.StartsWith("api/auth/", StringComparison.OrdinalIgnoreCase) => Auth,
            var path when path.StartsWith("api/members/", StringComparison.OrdinalIgnoreCase) => Members,
            var path when path.Equals("api/events", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("api/events/", StringComparison.OrdinalIgnoreCase) => Events,
            var path when path.Equals("api/inventory", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("api/inventory/", StringComparison.OrdinalIgnoreCase) => Inventory,
            _ => throw new InvalidOperationException($"No microservice is configured for '{relativeUri}'.")
        };

        return new Uri(root, relativeUri);
    }
}

public sealed class TunaEngineApiClient(HttpClient httpClient, ApiServiceEndpoints endpoints)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private string? _accessToken;

    public void SetAccessToken(string accessToken) =>
        Volatile.Write(ref _accessToken, accessToken);

    public void ClearAccessToken() => Volatile.Write(ref _accessToken, null);

    public async Task<T> GetAsync<T>(
        string relativeUri,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var uri = endpoints.Resolve(relativeUri);
        using var response = await SendAsync(HttpMethod.Get, uri, null, cancellationToken);
        await EnsureSuccessAsync(response, uri, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"The API returned an empty response for '{relativeUri}'.");
    }

    public async Task<T> PostAsync<T>(
        string relativeUri,
        object request,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var uri = endpoints.Resolve(relativeUri);
        using var response = await SendAsync(HttpMethod.Post, uri, request, cancellationToken);
        await EnsureSuccessAsync(response, uri, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"The API returned an empty response for '{relativeUri}'.");
    }

    public async Task<T> PutAsync<T>(
        string relativeUri,
        object request,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var uri = endpoints.Resolve(relativeUri);
        using var response = await SendAsync(HttpMethod.Put, uri, request, cancellationToken);
        await EnsureSuccessAsync(response, uri, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"The API returned an empty response for '{relativeUri}'.");
    }

    public async Task PutAsync(
        string relativeUri,
        object request,
        CancellationToken cancellationToken = default)
    {
        var uri = endpoints.Resolve(relativeUri);
        using var response = await SendAsync(HttpMethod.Put, uri, request, cancellationToken);
        await EnsureSuccessAsync(response, uri, cancellationToken);
    }

    public async Task DeleteAsync(
        string relativeUri,
        CancellationToken cancellationToken = default)
    {
        var uri = endpoints.Resolve(relativeUri);
        using var response = await SendAsync(HttpMethod.Delete, uri, null, cancellationToken);
        await EnsureSuccessAsync(response, uri, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        Uri uri,
        object? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        var accessToken = Volatile.Read(ref _accessToken);
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await httpClient.SendAsync(request, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        Uri uri,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(
            $"The API returned {(int)response.StatusCode} for '{uri.AbsolutePath}': {details}",
            null,
            response.StatusCode);
    }
}

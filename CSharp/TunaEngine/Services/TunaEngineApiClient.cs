using System.Net.Http.Json;

namespace TunaEngine.Services;

public sealed class TunaEngineApiClient(HttpClient httpClient)
{
    public async Task<T> GetAsync<T>(
        string relativeUri,
        CancellationToken cancellationToken = default)
        where T : class =>
        await httpClient.GetFromJsonAsync<T>(relativeUri, cancellationToken)
        ?? throw new InvalidOperationException(
            $"The API returned an empty response for '{relativeUri}'.");
}

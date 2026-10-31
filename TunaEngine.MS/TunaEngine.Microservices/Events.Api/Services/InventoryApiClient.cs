using System.Net;
using System.Net.Http.Headers;

namespace TunaEngine.Events.Api.Services;

public sealed class InventoryApiClient(HttpClient client)
{
    public async Task<bool> ItemExistsAsync(
        string inventoryId,
        string authorizationHeader,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/inventory/{Uri.EscapeDataString(inventoryId)}");
        if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorization))
        {
            request.Headers.Authorization = authorization;
        }

        using var response = await client.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}

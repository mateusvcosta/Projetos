using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using TunaEngine.Events.Api.Models;

namespace TunaEngine.Events.Api.Services;

public sealed class IpmaWeatherClient(HttpClient client, IMemoryCache cache)
{
    public async Task<IpmaDailyForecast?> GetForecastAsync(
        int globalIdLocal,
        DateOnly eventDate,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"ipma-forecast-{globalIdLocal}";
        if (!cache.TryGetValue(cacheKey, out IpmaForecastDocument? forecast))
        {
            forecast = await client.GetFromJsonAsync<IpmaForecastDocument>(
                $"open-data/forecast/meteorology/cities/daily/{globalIdLocal}.json",
                cancellationToken);

            if (forecast is not null)
            {
                cache.Set(cacheKey, forecast, TimeSpan.FromHours(6));
            }
        }

        return forecast?.Data.FirstOrDefault(day =>
            DateOnly.TryParseExact(
                day.ForecastDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var forecastDate)
            && forecastDate == eventDate);
    }
}

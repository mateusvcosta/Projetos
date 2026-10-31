using System.Text.Json.Serialization;

namespace TunaEngine.Events.Api.Models;

public sealed class IpmaForecastDocument
{
    [JsonPropertyName("data")]
    public List<IpmaDailyForecast> Data { get; init; } = [];
}

public sealed class IpmaDailyForecast
{
    [JsonPropertyName("forecastDate")]
    public string ForecastDate { get; init; } = "";

    [JsonPropertyName("tMin")]
    public string? MinimumTemperature { get; init; }

    [JsonPropertyName("tMax")]
    public string? MaximumTemperature { get; init; }

    [JsonPropertyName("precipitaProb")]
    public string? PrecipitationProbability { get; init; }

    [JsonPropertyName("predWindDir")]
    public string? PredominantWindDirection { get; init; }

    [JsonPropertyName("idWeatherType")]
    public int? WeatherTypeId { get; init; }

    [JsonPropertyName("classWindSpeed")]
    public int? WindSpeedClass { get; init; }
}

public sealed record EventWeatherResponse(
    string Source,
    string Location,
    int GlobalIdLocal,
    IpmaDailyForecast Forecast);

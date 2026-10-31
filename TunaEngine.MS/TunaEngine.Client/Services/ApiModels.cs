using System.Text.Json.Serialization;

namespace TunaEngine.Services;

public sealed class ApiInventoryItem
{
    public string Id { get; set; } = "";
    public string Designation { get; set; } = "";
    public string? InstrumentType { get; set; }
    public string? Brand { get; set; }
    public float? Price { get; set; }
    public string? Condition { get; set; }
    public string? Function { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

public sealed class ApiInventoryItemRequest
{
    public string Designation { get; init; } = "";
    public string? InstrumentType { get; init; }
    public string? Brand { get; init; }
    public float? Price { get; init; }
    public string? Condition { get; init; }
    public string? Function { get; init; }
    public string? Notes { get; init; }
}

public sealed class ApiLoginRequest
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}

public sealed class ApiRegisterRequest
{
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}

public sealed class ApiRegisteredUser
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
}

public sealed class ApiLoginResponse
{
    public string AccessToken { get; init; } = "";
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAtUtc { get; init; }
}

public sealed class ApiMemberProfile
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public string TunaName { get; init; } = "";
    public string? Phone { get; init; }
    public string? StudentNumber { get; init; }
    public string Role { get; init; } = "Membro";
    public string? VoiceType { get; init; }
    public string? Instrument { get; init; }
}

public sealed class ApiMemberProfileRequest
{
    public string TunaName { get; init; } = "";
    public string? Phone { get; init; }
    public string? StudentNumber { get; init; }
    public string? VoiceType { get; init; }
    public string? Instrument { get; init; }
}

public sealed class ApiEvent
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public string Location { get; set; } = "";

    [JsonPropertyName("date_Hour")]
    public DateTime Date { get; set; }

    public int? GlobalIdLocal { get; set; }
}

public sealed class ApiEventRequest
{
    public string Id_Event { get; init; } = "";
    public DateTimeOffset Date_Hour { get; init; }
    public string Name { get; init; } = "";
    public string Location { get; init; } = "";
    public int? GlobalIdLocal { get; init; }
}

public sealed class ApiEventWeather
{
    public string Source { get; init; } = "";
    public string Location { get; init; } = "";
    public int GlobalIdLocal { get; init; }
    public ApiWeatherForecast Forecast { get; init; } = new();
}

public sealed class ApiWeatherForecast
{
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


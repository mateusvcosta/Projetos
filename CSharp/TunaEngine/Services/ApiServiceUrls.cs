using Microsoft.Maui.Storage;

namespace TunaEngine.Services;

public static class ApiServiceUrls
{
    public static string Gateway =>
        Environment.GetEnvironmentVariable("TUNAENGINE_GATEWAY_URL")
        ?? Preferences.Get("TUNAENGINE_GATEWAY_URL", "http://localhost:8080/");
}

namespace TunaEngine.Api.Models;

public sealed class LoginRequest
{
    public string? Email { get; init; }
    public string? Password { get; init; }

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Email))
        {
            errors[nameof(Email)] = ["Email is required."];
        }

        if (string.IsNullOrEmpty(Password))
        {
            errors[nameof(Password)] = ["Password is required."];
        }

        return errors;
    }
}

using System.ComponentModel.DataAnnotations;

namespace TunaEngine.Auth.Api.Models;

public sealed class RegisterRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string? Name { get; init; }

    [Required, EmailAddress, StringLength(254)]
    public string? Email { get; init; }

    [Required, StringLength(128, MinimumLength = 8)]
    public string? Password { get; init; }
}

public sealed class LoginRequest
{
    [Required, EmailAddress, StringLength(254)]
    public string? Email { get; init; }

    [Required, StringLength(128, MinimumLength = 8)]
    public string? Password { get; init; }
}

public sealed record RegisteredUserResponse(string Id, string Name, string Email);

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);

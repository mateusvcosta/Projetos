namespace TunaEngine.Api.Models;

public sealed record LoginResponse(
    string Id,
    string Name,
    string Email,
    string? RoleId);

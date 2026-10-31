using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using TunaEngine.Auth.Api.Models;

namespace TunaEngine.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IMongoCollection<AuthUser> users,
    IPasswordHasher<AuthUser> passwordHasher,
    JwtSettings jwtSettings) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<RegisteredUserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisteredUserResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email!.Trim().ToLowerInvariant();
        var user = new AuthUser
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Name = request.Name!.Trim(),
            Email = email
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password!);

        try
        {
            await users.InsertOneAsync(user, cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new ProblemDetails { Title = "An account with this email already exists." });
        }

        return StatusCode(StatusCodes.Status201Created,
            new RegisteredUserResponse(user.Id, user.Name, user.Email));
    }

    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email!.Trim().ToLowerInvariant();
        var user = await users.Find(candidate => candidate.Email == email)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null || passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password!) == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new ProblemDetails { Title = "Invalid email or password." });
        }

        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(1);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        return Ok(new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiresAt));
    }
}

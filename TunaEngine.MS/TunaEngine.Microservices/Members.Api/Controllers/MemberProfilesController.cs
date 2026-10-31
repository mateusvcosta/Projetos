using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Members.Api.Models;
using TunaEngine.Members.Api.Services;

namespace TunaEngine.Members.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/members/me")]
public sealed class MemberProfilesController(MemberProfileService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<MemberProfile>> GetMe(CancellationToken cancellationToken)
    {
        var id = GetMemberId();
        if (id is null)
        {
            return Unauthorized();
        }

        var profile = await service.GetById(id, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<MemberProfile>> UpsertMe(
        MemberProfileRequest request,
        CancellationToken cancellationToken)
    {
        var id = GetMemberId();
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? User.FindFirstValue(ClaimTypes.Email);

        if (id is null || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var profile = await service.Upsert(id, name, email, request, cancellationToken);
        return Ok(profile);
    }

    private string? GetMemberId()
    {
        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return ObjectId.TryParse(id, out _) ? id : null;
    }
}

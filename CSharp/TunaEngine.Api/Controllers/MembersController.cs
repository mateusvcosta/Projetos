using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Api.Interfaces; 
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Controllers;

[ApiController]
[Route("api/members")]

public sealed class MembersController(MembersInterface membersService):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Members>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await membersService.GetAllMembers(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Members>> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The members id must be a MongoDB ObjectId." });
        }

        var item = await membersService.GetByIdMember(id, cancellationToken);

        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var member = await membersService.Login(
            request.Email!,
            request.Password!,
            cancellationToken);

        return member is null
            ? Unauthorized(new { error = "Invalid email or password." })
            : Ok(new LoginResponse(member.Id, member.Name, member.Email, member.RoleId));
    }

    [HttpPost]
    public async Task<ActionResult<Members>> Create(
        [FromBody] MembersRequest request,
        CancellationToken cancellationToken)
    {
        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var item = await membersService.CreateMember(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] MembersRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The members id must be a MongoDB ObjectId." });
        }

        var errors = request.GetValidationErrors(requirePassword: false);
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var updated = await membersService.UpdateMember(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The members id must be a MongoDB ObjectId." });
        }

        var deleted = await membersService.DeleteMember(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

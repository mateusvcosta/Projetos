using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Controllers;

[ApiController]
[Route("api/roles")]
public sealed class RolesController(RolesInterface rolesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Roles>>> GetAll(
        CancellationToken cancellationToken)
    {
        var roles = await rolesService.GetAllRoles(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Roles>> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The role id must be a MongoDB ObjectId." });
        }

        var role = await rolesService.GetByIdRole(id, cancellationToken);

        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Roles>> Create(
        [FromBody] RolesRequest request,
        CancellationToken cancellationToken)
    {
        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var role = await rolesService.CreateRole(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] RolesRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The role id must be a MongoDB ObjectId." });
        }

        var updated = await rolesService.UpdateRole(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The role id must be a MongoDB ObjectId." });
        }

        var deleted = await rolesService.DeleteRole(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

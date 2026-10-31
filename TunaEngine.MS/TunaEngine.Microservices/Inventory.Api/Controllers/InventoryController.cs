using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Inventory.Api.Models;
using TunaEngine.Inventory.Api.Services;

namespace TunaEngine.Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/inventory")]
public sealed class InventoryController(InventoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItem>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAll(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Inventory id must be a MongoDB ObjectId." });
        }

        var item = await service.GetById(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> Create(
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var item = await service.Create(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Inventory id must be a MongoDB ObjectId." });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        return await service.Update(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Inventory id must be a MongoDB ObjectId." });
        }

        return await service.Delete(id, cancellationToken) ? NoContent() : NotFound();
    }
}

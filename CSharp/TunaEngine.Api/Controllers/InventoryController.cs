using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController(InventoryInterface inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItem>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await inventoryService.GetAllInv(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The inventory id must be a MongoDB ObjectId." });
        }

        var item = await inventoryService.GetByIdInv(id, cancellationToken);

        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> Create(
        [FromBody] InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var item = await inventoryService.CreateItem(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The inventory id must be a MongoDB ObjectId." });
        }

        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var updated = await inventoryService.UpdateItem(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The inventory id must be a MongoDB ObjectId." });
        }

        var deleted = await inventoryService.DeleteItem(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

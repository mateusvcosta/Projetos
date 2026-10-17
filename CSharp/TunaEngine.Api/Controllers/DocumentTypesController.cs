using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Controllers;

[ApiController]
[Route("api/doc-types")]
public sealed class DocumentTypesController(DocumentTypesInterface service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DocumentType>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAll(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentType>> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The document type id must be a MongoDB ObjectId." });
        }

        var item = await service.GetById(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DocumentType>> Create(
        [FromBody] DocumentTypeRequest request,
        CancellationToken cancellationToken)
    {
        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        var item = await service.Create(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id, [FromBody] DocumentTypeRequest request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The document type id must be a MongoDB ObjectId." });
        }

        var errors = request.GetValidationErrors();
        if (errors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(errors));
        }

        return await service.Update(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "The document type id must be a MongoDB ObjectId." });
        }

        return await service.Delete(id, cancellationToken) ? NoContent() : NotFound();
    }
}

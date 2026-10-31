using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TunaEngine.Events.Api.Models;
using TunaEngine.Events.Api.Services;
using EventDocument = TunaEngine.Events.Api.Models.Events;

namespace TunaEngine.Events.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/events")]
public sealed class EventsController(
    EventsService service,
    InventoryApiClient inventoryApi,
    IpmaWeatherClient weatherClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventDocument>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAll(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<EventDocument>> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Event id must be a MongoDB ObjectId." });
        }

        var item = await service.GetById(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id}/weather")]
    public async Task<ActionResult<EventWeatherResponse>> GetWeather(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Event id must be a MongoDB ObjectId." });
        }

        var item = await service.GetById(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        if (item.GlobalIdLocal is null or <= 0)
        {
            return BadRequest(new { error = "This event has no valid IPMA location code." });
        }

        try
        {
            var forecast = await weatherClient.GetForecastAsync(
                item.GlobalIdLocal.Value,
                DateOnly.FromDateTime(item.Date_Hour),
                cancellationToken);

            if (forecast is null)
            {
                return NotFound(new { error = "IPMA has no forecast for this event date and location." });
            }

            return Ok(new EventWeatherResponse(
                "IPMA", item.Location, item.GlobalIdLocal.Value, forecast));
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "IPMA weather service is temporarily unavailable." });
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "IPMA weather service timed out." });
        }
    }

    [HttpPost]
    public async Task<ActionResult<EventDocument>> Create(EventsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id_Event)
            || request.Date_Hour is null
            || string.IsNullOrWhiteSpace(request.Name)
            || string.IsNullOrWhiteSpace(request.Location)
            || request.Instruments_Needed.Any(item => item.Quantity < 0)
            || request.Boleias.Any(item => item.Available_Seats < 0))
        {
            return BadRequest(new { error = "Event details are missing or contain invalid quantities." });
        }

        var inventoryError = await ValidateInventoryReferences(request, cancellationToken);
        if (inventoryError is not null)
        {
            return inventoryError;
        }

        var item = await service.Create(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPost("{id}/attendance")]
    public async Task<ActionResult<EventDocument>> SetAttendance(
        string id,
        AttendanceRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Event id must be a MongoDB ObjectId." });
        }

        if (request.Status is not ("going" or "not-going"))
        {
            return BadRequest(new { error = "Status must be 'going' or 'not-going'." });
        }

        var memberId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(memberId))
        {
            return Unauthorized();
        }

        var item = await service.SetAttendance(id, memberId, request.Status, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        EventsRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Event id must be a MongoDB ObjectId." });
        }

        if (string.IsNullOrWhiteSpace(request.Id_Event)
            || request.Date_Hour is null
            || string.IsNullOrWhiteSpace(request.Name)
            || string.IsNullOrWhiteSpace(request.Location))
        {
            return BadRequest(new { error = "Event details are required." });
        }

        var inventoryError = await ValidateInventoryReferences(request, cancellationToken);
        if (inventoryError is not null)
        {
            return inventoryError;
        }

        return await service.Update(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest(new { error = "Event id must be a MongoDB ObjectId." });
        }

        return await service.Delete(id, cancellationToken) ? NoContent() : NotFound();
    }

    private async Task<ActionResult?> ValidateInventoryReferences(
        EventsRequest request,
        CancellationToken cancellationToken)
    {
        var authorization = Request.Headers.Authorization.ToString();
        foreach (var instrument in request.Instruments_Needed)
        {
            if (!ObjectId.TryParse(instrument.Id_Inventory, out _))
            {
                return BadRequest(new { error = "Each instrument must reference a valid inventory ObjectId." });
            }

            try
            {
                if (!await inventoryApi.ItemExistsAsync(
                        instrument.Id_Inventory,
                        authorization,
                        cancellationToken))
                {
                    return BadRequest(new { error = $"Inventory item '{instrument.Id_Inventory}' does not exist." });
                }
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "Inventory service is temporarily unavailable." });
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "Inventory service timed out." });
            }
        }

        return null;
    }
}

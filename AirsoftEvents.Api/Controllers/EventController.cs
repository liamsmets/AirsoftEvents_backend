using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Exceptions;
using AirsoftEvents.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AirsoftEvents.Api.Extensions;


namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventService _service) : ControllerBase
{
    [Authorize(Policy = "ApiWritePolicy")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequestContract eventToCreate)
    {
        var ownerId = User.GetUserId();

        try
        {
            var createdEvent = await _service.CreateEventAsync(eventToCreate, ownerId);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex) when (ex.GetType().Name == "TerrainNotApprovedException" || ex.GetType().Name == "CapacityExceededException")
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        var events = await _service.GetUpcomingEventsAsync(EventStatus.Approved);
        return Ok(events);
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyEvents()
    {
        var ownerId = User.GetUserId();

        var events = await _service.GetMyEventsAsync(ownerId);
        return Ok(events);
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _service.GetAllEventsAsync();
        return Ok(events);
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById([FromRoute] Guid id)
    {
        var eventItem = await _service.GetEventByIdAsync(id);

        if (eventItem == null)
        {
            return NotFound();
        }

        return Ok(eventItem);
    }

    [Authorize(Policy = "ApiAdminPolicy")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveEvent([FromRoute] Guid id)
    {
        try
        {
            await _service.ApproveEventAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Services.Interfaces;
using AirsoftEvents.Core.Exceptions;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;

    public EventsController(IEventService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequestContract eventToCreate)
    {
        try
        {
            var createdEvent = await _service.CreateEventAsync(eventToCreate);
            
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
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

    [HttpGet]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        var events = await _service.GetUpcomingEventsAsync();
        return Ok(events);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _service.GetAllEventsAsync();
        return Ok(events);
    }

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
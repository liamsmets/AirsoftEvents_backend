using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Exceptions;
using AirsoftEvents.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AirsoftEvents.Api.Extensions;
using CsvHelper;
using System.Text;
using System.Globalization;


namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventService _service, IWeatherService _weatherService) : ControllerBase
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
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        var events = await _service.GetUpcomingEventsAsync();
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

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById([FromRoute] Guid id)
    {
        var eventItem = await _service.GetEventByIdAsync(id);
        if (eventItem is null) return NotFound();

        return Ok(eventItem);
    }

    [Authorize(Policy = "ApiAdminPolicy")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveEvent([FromRoute] Guid id)
    {
        await _service.ApproveEventAsync(id);
        return NoContent();

    }

    [Authorize(Policy = "ApiWritePolicy")]
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectEvent(Guid id)
    {
        await _service.RejectEventAsync(id);
        return NoContent();
    }

    [Authorize(Policy = "ApiWritePolicy")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent([FromRoute] Guid id, [FromBody] EventUpdateContract update)
    {
        var organizerId = User.GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var updated = await _service.UpdateEventAsync(id, update, organizerId, isAdmin);
        return Ok(updated);

    }

    [Authorize(Policy = "ApiWritePolicy")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent([FromRoute] Guid id)
    {
        var organizerId = User.GetUserId();
        var isAdmin = User.IsInRole("Admin");

        await _service.DeleteEventAsync(id, organizerId, isAdmin);
        return NoContent();

    }

    [AllowAnonymous]
    [HttpGet("{id}/availability")]
    public async Task<IActionResult> GetAvailability([FromRoute] Guid id)
    {
        var dto = await _service.GetAvailabilityAsync(id);
        return Ok(dto);

    }

    [AllowAnonymous]
    [HttpGet("{eventId}/weather")]
    public async Task<IActionResult> GetWeatherForEvent([FromRoute] Guid eventId)
    {
        var ev = await _service.GetEventByIdAsync(eventId);
        if (ev is null) return NotFound();

        const double lat = 50.9368;
        const double lon = 4.0397;

        var weather = await _weatherService.GetWeatherForDateAsync(ev.Date, lat, lon);
        if (weather is null) return NoContent();

        return Ok(weather);
    }

    [AllowAnonymous]
    [HttpGet("weather")]
    public async Task<IActionResult> GetWeatherPreview([FromQuery] DateTime datum)
    {
        if (datum == default) return BadRequest("Query parameter 'datum' is verplicht (YYYY-MM-DD).");

        const double lat = 50.9368;
        const double lon = 4.0397;

        var weather = await _weatherService.GetWeatherForDateAsync(datum.Date, lat, lon);

        if (weather is null) return NoContent();

        return Ok(weather);
    }

    [AllowAnonymous]
    [HttpGet("upcoming/export")]
    public async Task<IActionResult> ExportUpcomingEvents([FromQuery] string? searchTerm)
    {
        var events = await _service.GetUpcomingEventsAsync();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            events = events.Where(e => e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var memoryStream = new MemoryStream();

        var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
        var csvWriter = new CsvWriter(streamWriter, new CultureInfo("nl-BE"));

        var exportData = events.Select(e => new
        {
            Id = e.Id,
            Naam = e.Name,
            Datum = e.Date.ToShortDateString(),
            Prijs = e.Price,
            Locatie = e.FieldId,
            Beschrijving = e.Description
        });

        await csvWriter.WriteRecordsAsync(exportData);

        await streamWriter.FlushAsync();

        memoryStream.Position = 0;

        return File(memoryStream, "text/csv", $"upcoming_events_{DateTime.Now:yyyyMMdd}.csv");
    }
}
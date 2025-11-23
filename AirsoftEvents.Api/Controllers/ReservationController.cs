using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Services.Interfaces;
using AirsoftEvents.Core.Exceptions; // Zorg dat deze erbij staat voor de exceptions!

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _service;

    public ReservationsController(IReservationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestContract reservationToCreate)
    {
        try
        {
            var createdReservation = await _service.CreateReservationAsync(reservationToCreate);
            
            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.Id }, createdReservation);
        }
        catch (CapacityExceededException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservationById([FromRoute] Guid id)
    {
        var reservation = await _service.GetReservationByIdAsync(id);

        if (reservation == null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }
}
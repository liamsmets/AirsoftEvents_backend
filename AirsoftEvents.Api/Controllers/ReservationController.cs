using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Api.Options;
using AirsoftEvents.Api.Payments;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Persistance.Entities;
using AirsoftEvents.Persistance.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationRepo _reservationRepo;
    private readonly IEventService _eventService;
    private readonly MockMollieStore _mockMollieStore;
    private readonly MollieOptions _mollieOptions;

    public ReservationsController(
        IReservationRepo reservationRepo,
        IEventService eventService,
        MockMollieStore mockMollieStore,
        IOptions<MollieOptions> mollieOptions)
    {
        _reservationRepo = reservationRepo;
        _eventService = eventService;
        _mockMollieStore = mockMollieStore;
        _mollieOptions = mollieOptions.Value;
    }

    // ---------------------------
    // GET api/reservations/{id}
    // ---------------------------
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var r = await _reservationRepo.GetByIdAsync(id);
        if (r == null) return NotFound();

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        // enkel eigenaar of admin
        if (!isAdmin && tokenUserId.Value != r.UserId)
            return Forbid();

        return Ok(new
        {
            id = r.Id,
            eventId = r.EventId,
            userId = r.UserId,
            reservedAt = r.ReservedAt,
            paymentStatus = r.PaymentStatus.ToString(),
            molliePaymentId = r.MolliePaymentId
        });
    }

    // ---------------------------
    // POST api/reservations
    // body: { eventId, userId }
    // ---------------------------
    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationRequestContract body)
    {
        if (body.EventId == Guid.Empty || body.UserId == Guid.Empty)
            return BadRequest("eventId en userId zijn verplicht.");

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        // user mag enkel voor zichzelf reserveren (tenzij admin)
        if (!isAdmin && tokenUserId.Value != body.UserId)
            return Forbid();

        // availability check via EventService
        var availability = await _eventService.GetAvailabilityAsync(body.EventId);
        if (availability.Free <= 0)
            return BadRequest("Event is volzet.");

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            EventId = body.EventId,
            UserId = body.UserId,
            ReservedAt = DateTime.UtcNow,
            PaymentStatus = ReservationpaymentStatus.Pending
        };

        await _reservationRepo.AddAsync(reservation);

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, new
        {
            id = reservation.Id,
            eventId = reservation.EventId,
            userId = reservation.UserId,
            reservedAt = reservation.ReservedAt,
            paymentStatus = reservation.PaymentStatus.ToString()
        });
    }

    // ---------------------------
    // POST api/reservations/{id}/pay
    // ---------------------------
    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<StartPaymentResponseContract>> StartPayment([FromRoute] Guid id)
    {
        var reservation = await _reservationRepo.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        // enkel eigenaar of admin
        if (!isAdmin && tokenUserId.Value != reservation.UserId)
            return Forbid();

        if (reservation.PaymentStatus == ReservationpaymentStatus.paid)
            return BadRequest("Reservatie is al betaald.");

        // mock payment
        var payment = _mockMollieStore.Create(reservation.Id);

        reservation.MolliePaymentId = payment.PaymentId;
        reservation.PaymentStatus = ReservationpaymentStatus.paymentCreated;
        await _reservationRepo.UpdateAsync(reservation);

        var checkoutUrl =
            $"{_mollieOptions.RedirectBaseUrl}/payment/mock" +
            $"?reservationId={reservation.Id}&paymentId={payment.PaymentId}";

        return Ok(new StartPaymentResponseContract
        {
            CheckoutUrl = checkoutUrl,
            PaymentId = payment.PaymentId
        });
    }

    // ---------------------------
    // Helper: haal GUID userId uit token claims
    // ---------------------------
    private Guid? GetUserIdFromClaims()
    {
        // Probeer veelvoorkomende claims. Kies degene die jij ook bij events gebruikt.
        var candidates = new[]
        {
            User.FindFirst("userId")?.Value, // als jij deze claim zelf toevoegt
            User.FindFirst("sub")?.Value,
            User.FindFirst("oid")?.Value,
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
        };

        foreach (var c in candidates)
        {
            if (!string.IsNullOrWhiteSpace(c) && Guid.TryParse(c, out var g))
                return g;
        }

        return null;
    }
}

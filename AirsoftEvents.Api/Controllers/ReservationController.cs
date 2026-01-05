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
using System.Net.Http.Json;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationRepo _reservationRepo;
    private readonly IEventService _eventService;
    private readonly MockMollieStore _mockMollieStore;
    private readonly MollieOptions _mollieOptions;

    // ✅ nieuw
    private readonly IHttpClientFactory _httpClientFactory;

    public ReservationsController(
        IReservationRepo reservationRepo,
        IEventService eventService,
        MockMollieStore mockMollieStore,
        IOptions<MollieOptions> mollieOptions,
        IHttpClientFactory httpClientFactory 
    )
    {
        _reservationRepo = reservationRepo;
        _eventService = eventService;
        _mockMollieStore = mockMollieStore;
        _mollieOptions = mollieOptions.Value;
        _httpClientFactory = httpClientFactory; 
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var r = await _reservationRepo.GetByIdAsync(id);
        if (r == null) return NotFound();

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && tokenUserId.Value != r.UserId)
            return Forbid();

        object? emailPreview = null;

        if (r.PaymentStatus == ReservationpaymentStatus.paid)
        {
            var email = User.FindFirst("email")?.Value;

            if (!string.IsNullOrWhiteSpace(email))
            {
                emailPreview = new
                {
                    to = email,
                    subject = "Reservatie bevestigd",
                    body = $"Je reservatie {r.Id} is betaald en bevestigd."
                };
            }
        }

        return Ok(new
        {
            id = r.Id,
            eventId = r.EventId,
            userId = r.UserId,
            reservedAt = r.ReservedAt,
            paymentStatus = r.PaymentStatus.ToString(),
            molliePaymentId = r.MolliePaymentId,
            emailPreview
        });
    }

    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationRequestContract body)
    {
        if (body.EventId == Guid.Empty || body.UserId == Guid.Empty)
            return BadRequest("eventId en userId zijn verplicht.");

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && tokenUserId.Value != body.UserId)
            return Forbid();

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

    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<StartPaymentResponseContract>> StartPayment([FromRoute] Guid id)
    {
        var reservation = await _reservationRepo.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && tokenUserId.Value != reservation.UserId)
            return Forbid();

        if (reservation.PaymentStatus == ReservationpaymentStatus.paid)
            return BadRequest("Reservatie is al betaald.");

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

    private Guid? GetUserIdFromClaims()
    {
        var candidates = new[]
        {
            User.FindFirst("userId")?.Value,
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

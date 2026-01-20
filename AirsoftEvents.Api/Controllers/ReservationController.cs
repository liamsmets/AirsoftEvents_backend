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
    private readonly IReservationService _reservationService;
    private readonly IEventService _eventService;
    private readonly MockMollieStore _mockMollieStore;
    private readonly MollieOptions _mollieOptions;

    public ReservationsController(
        IReservationService reservationService,
        IEventService eventService,
        MockMollieStore mockMollieStore,
        IOptions<MollieOptions> mollieOptions
    )
    {
        _reservationService = reservationService;
        _eventService = eventService;
        _mockMollieStore = mockMollieStore;
        _mollieOptions = mollieOptions.Value;
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var r = await _reservationService.GetReservationByIdAsync(id);
        if (r == null) return NotFound();

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && tokenUserId.Value != r.UserId)
            return Forbid();


        return Ok(new
        {
            id = r.Id,
            eventId = r.EventId,
            userId = r.UserId,
            reservedAt = r.ReservedAt,
            paymentStatus = r.PaymentStatus.ToString(),
            molliePaymentId = r.MolliePaymentId,
        });
    }

    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationRequestContract body)
    {
        
        if (body.EventId == Guid.Empty)
            return BadRequest("eventId is verplicht.");

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized("No valid user id in token.");

        var availability = await _eventService.GetAvailabilityAsync(body.EventId);
        if (availability.Free <= 0)
            return BadRequest("Event is volzet.");

        var userId = tokenUserId;

        var created = await _reservationService.CreateReservationAsync(body, tokenUserId.Value);
        var mockPayment = _mockMollieStore.Create(created.Id);
        await _reservationService.UpdatePaymentStatusAsync(
             created.Id, 
             mockPayment.PaymentId, 
             ReservationpaymentStatus.paid
        );

        created.MolliePaymentId = mockPayment.PaymentId; 
        created.PaymentStatus = ReservationpaymentStatus.paid;

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<StartPaymentResponseContract>> StartPayment([FromRoute] Guid id)
    {
        var reservationContract = await _reservationService.GetReservationByIdAsync(id);
        if (reservationContract == null) return NotFound();

        var tokenUserId = GetUserIdFromClaims();
        if (tokenUserId == null) return Unauthorized();
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && tokenUserId.Value != reservationContract.UserId) return Forbid();

        if (reservationContract.PaymentStatus == ReservationpaymentStatus.paid) 
            return BadRequest("Reservatie is al betaald.");

        var payment = _mockMollieStore.Create(reservationContract.Id);

        await _reservationService.UpdatePaymentStatusAsync(reservationContract.Id, payment.PaymentId, ReservationpaymentStatus.paymentCreated);

        var checkoutUrl = $"{_mollieOptions.RedirectBaseUrl}/payment/checkout?reservationId={reservationContract.Id}&paymentId={payment.PaymentId}";

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

using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment.Request;
using AirsoftEvents.Api.Options;
using AirsoftEvents.Api.Extensions;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly IEventService _eventService;
    private readonly IPaymentClient _molliePaymentClient;
    private readonly MollieOptions _mollieOptions;

    public ReservationsController(
        IReservationService reservationService,
        IEventService eventService,
        IPaymentClient molliePaymentClient,
        IOptions<MollieOptions> mollieOptions
    )
    {
        _reservationService = reservationService;
        _eventService = eventService;
        _molliePaymentClient = molliePaymentClient;
        _mollieOptions = mollieOptions.Value;
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var r = await _reservationService.GetReservationByIdAsync(id);
        if (r == null) return NotFound();

        var tokenUserId = User.GetUserId();
        if (tokenUserId == Guid.Empty) return Unauthorized("No valid user id in token.");

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && tokenUserId != r.UserId)
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

        var tokenUserId = User.GetUserId();
        if (tokenUserId == Guid.Empty) return Unauthorized("No valid user id in token.");

        var availability = await _eventService.GetAvailabilityAsync(body.EventId);
        if (availability.Free <= 0)
            return BadRequest("Event is volzet.");

        var created = await _reservationService.CreateReservationAsync(body, tokenUserId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Policy = "ApiUserWritePolicy")]
    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<StartPaymentResponseContract>> StartPayment([FromRoute] Guid id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null) return NotFound();

        var tokenUserId = User.GetUserId();
        if (tokenUserId == Guid.Empty) return Unauthorized();
        
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && tokenUserId != reservation.UserId) return Forbid();

        if (reservation.PaymentStatus == ReservationpaymentStatus.paid)
            return BadRequest("Reservatie is al betaald.");

        var eventItem = await _eventService.GetEventByIdAsync(reservation.EventId);

        var paymentRequest = new PaymentRequest()
        {
            Amount = new Amount(Currency.EUR, eventItem!.Price),
            Description = $"Reservering {eventItem.Name}",
            RedirectUrl = $"{_mollieOptions.RedirectBaseUrl}/payment/return?reservationId={id}",
            WebhookUrl = _mollieOptions.WebhookUrl,
            Metadata = id.ToString()
        };

        var mollieResponse = await _molliePaymentClient.CreatePaymentAsync(paymentRequest);

        await _reservationService.UpdatePaymentStatusAsync(
            reservation.Id,
            mollieResponse.Id,
            ReservationpaymentStatus.paymentCreated
        );

        return Ok(new StartPaymentResponseContract
        {
            CheckoutUrl = mollieResponse.Links.Checkout?.Href ?? "",
            PaymentId = mollieResponse.Id
        });
    }
}

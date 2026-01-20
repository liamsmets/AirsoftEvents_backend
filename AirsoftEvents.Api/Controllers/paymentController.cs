using AirsoftEvents.Api.Options;
using AirsoftEvents.Api.Payments;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Persistance.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using System.Net.Http.Headers;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly MockMollieStore _mockMollieStore;
    private readonly MollieOptions _mollieOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentsController(
        IReservationService reservationService,
        MockMollieStore mockMollieStore,
        IOptions<MollieOptions> mollieOptions,
        IHttpClientFactory httpClientFactory)
    {
        _reservationService = reservationService;
        _mockMollieStore = mockMollieStore;
        _mollieOptions = mollieOptions.Value;
        _httpClientFactory = httpClientFactory;
    }

    [AllowAnonymous]
    [HttpPost("mollie/webhook")]
    public async Task<IActionResult> MollieWebhook([FromQuery] string secret, [FromForm] string id,[FromQuery] string? email)
    {
        if (secret != _mollieOptions.WebhookSecret) return Unauthorized();

        var mockPayment = _mockMollieStore.Get(id);
        if (mockPayment == null) return Ok();

        var reservation = await _reservationService.GetByMolliePaymentIdAsync(id);
        if (reservation == null) return Ok();

        reservation.PaymentStatus = mockPayment.Status switch
        {
            MockMolliePaymentStatus.Paid => ReservationpaymentStatus.paid,
            MockMolliePaymentStatus.Canceled => ReservationpaymentStatus.Canceled,
            _ => reservation.PaymentStatus
        };

        await _reservationService.UpdatePaymentStatusAsync(reservation.Id, reservation.MolliePaymentId, reservation.PaymentStatus);
        
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("mollie/mock/trigger")]
    public async Task<IActionResult> MockTrigger([FromQuery] string paymentId, [FromQuery] string status)
    {
        var newStatus = status.ToLower() == "paid" ? MockMolliePaymentStatus.Paid : MockMolliePaymentStatus.Canceled;

        _mockMollieStore.UpdateStatus(paymentId, newStatus);

        var client = _httpClientFactory.CreateClient("MollieLocal");

        var webhookUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/payments/mollie/webhook";
        
        await client.PostAsync(webhookUrl, new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("id", paymentId)
        }));

        return Ok("Webhook aangeroepen");
    }
}

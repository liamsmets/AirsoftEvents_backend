using AirsoftEvents.Api.Options;
using AirsoftEvents.Api.Payments;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Persistance.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IReservationRepo _reservationRepo;
    private readonly MockMollieStore _mockMollieStore;
    private readonly MollieOptions _mollieOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserRepo _userRepo;
    

    public PaymentsController(
        IReservationRepo reservationRepo,
        MockMollieStore mockMollieStore,
        IOptions<MollieOptions> mollieOptions,
        IHttpClientFactory httpClientFactory, IUserRepo userRepo)
    {
        _reservationRepo = reservationRepo;
        _mockMollieStore = mockMollieStore;
        _mollieOptions = mollieOptions.Value;
        _httpClientFactory = httpClientFactory;
        _userRepo = userRepo;
    }

    [AllowAnonymous]
    [HttpPost("mollie/webhook")]
    public async Task<IActionResult> MollieWebhook([FromQuery] string secret, [FromForm] string id,[FromQuery] string? email)
    {
        if (secret != _mollieOptions.WebhookSecret) return Unauthorized();

        var mockPayment = _mockMollieStore.Get(id);
        if (mockPayment == null) return Ok();

        var reservation = await _reservationRepo.GetByMolliePaymentIdAsync(id);
        if (reservation == null) return Ok();

        reservation.PaymentStatus = mockPayment.Status switch
        {
            MockMolliePaymentStatus.Paid => ReservationpaymentStatus.paid,
            MockMolliePaymentStatus.Failed => ReservationpaymentStatus.Failed,
            MockMolliePaymentStatus.Canceled => ReservationpaymentStatus.Canceled,
            MockMolliePaymentStatus.Expired => ReservationpaymentStatus.Expired,
            _ => reservation.PaymentStatus
        };

        await _reservationRepo.UpdateAsync(reservation);
        

        if (reservation.PaymentStatus == ReservationpaymentStatus.paid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("NotificationApi");

                var payload = new
                {
                    To = email,
                    subject = "Reservation created",
                    body = $"Reservation {reservation.Id} for event {reservation.EventId} was created."
                };

                await client.PostAsJsonAsync("/notifications/email", payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotificationApi] Failed to send fake email: {ex.Message}");
            }
        }
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("mollie/mock/trigger")]
    public async Task<IActionResult> MockTrigger([FromQuery] string paymentId, [FromQuery] string status, [FromQuery] string email)
    {
        var mapped = status.ToLowerInvariant() switch
        {
            "paid" => MockMolliePaymentStatus.Paid,
            "failed" => MockMolliePaymentStatus.Failed,
            "canceled" => MockMolliePaymentStatus.Canceled,
            "expired" => MockMolliePaymentStatus.Expired,
            _ => MockMolliePaymentStatus.Open
        };

        var ok = _mockMollieStore.UpdateStatus(paymentId, mapped);
        if (!ok) return NotFound("Unknown paymentId");

        var client = _httpClientFactory.CreateClient();

        var req = HttpContext.Request;
        var requestBase = $"{req.Scheme}://{req.Host}";

        var backendBase = string.IsNullOrWhiteSpace(_mollieOptions.BackendBaseUrl)
            ? requestBase
            : _mollieOptions.BackendBaseUrl.TrimEnd('/');

        var webhookUrl =
            $"{backendBase}/api/payments/mollie/webhook" +
            $"?secret={_mollieOptions.WebhookSecret}" +
            $"&email={Uri.EscapeDataString(email)}";

        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("id", paymentId)
        });
        var resp = await client.PostAsync(webhookUrl, form);

        return Ok(new { webhookCalled = resp.IsSuccessStatusCode });
    }
}

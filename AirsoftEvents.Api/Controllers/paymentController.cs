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

    public PaymentsController(
        IReservationRepo reservationRepo,
        MockMollieStore mockMollieStore,
        IOptions<MollieOptions> mollieOptions,
        IHttpClientFactory httpClientFactory)
    {
        _reservationRepo = reservationRepo;
        _mockMollieStore = mockMollieStore;
        _mollieOptions = mollieOptions.Value;
        _httpClientFactory = httpClientFactory;
    }

    // === 1) WEBHOOK (zoals echte Mollie) ===
    // Mollie zou een form-post doen met "id=<paymentId>"
    [AllowAnonymous]
    [HttpPost("mollie/webhook")]
    public async Task<IActionResult> MollieWebhook([FromQuery] string secret, [FromForm] string id)
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
        return Ok();
    }

    // === 2) MOCK PROVIDER TRIGGER (dit simuleert Mollie checkout) ===
    // frontend mock checkout roept dit aan met status (Paid/Failed/...)
    // daarna post dit endpoint naar de webhook endpoint
    [AllowAnonymous]
    [HttpPost("mollie/mock/trigger")]
    public async Task<IActionResult> MockTrigger([FromQuery] string paymentId, [FromQuery] string status)
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

        // DOE ECHT EEN HTTP POST NAAR JE WEBHOOK (zodat je webhook bewezen werkt)
        var client = _httpClientFactory.CreateClient();

        var webhookUrl =
            $"{_mollieOptions.BackendBaseUrl}/api/payments/mollie/webhook?secret={_mollieOptions.WebhookSecret}";

        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("id", paymentId)
        });

        // sommige setups hebben self-signed dev cert issues met https->https.
        // Als je daar last van hebt: zet BackendBaseUrl tijdelijk op http.
        var resp = await client.PostAsync(webhookUrl, form);

        // altijd ok terug naar frontend
        return Ok(new { webhookCalled = resp.IsSuccessStatusCode });
    }
}

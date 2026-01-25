using Microsoft.AspNetCore.Mvc;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models.Payment;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly IPaymentClient _paymentClient;

    public WebhooksController(IReservationService reservationService, IPaymentClient paymentClient)
    {
        _reservationService = reservationService;
        _paymentClient = paymentClient;
    }

    [HttpPost("mollie")]
    public async Task<IActionResult> HandleMollieWebhook([FromForm] string id)
    {
        var payment = await _paymentClient.GetPaymentAsync(id);

        if (payment.Status == PaymentStatus.Paid)
        {
            if (Guid.TryParse(payment.Metadata, out Guid reservationId))
            {
                await _reservationService.UpdatePaymentStatusAsync(
                reservationId,
                payment.Id,
                ReservationpaymentStatus.paid
            );
                Console.WriteLine($"Betaling ontvangen voor reservering: {reservationId}");
            }
        }
        else if (payment.Status == PaymentStatus.Canceled)
        {
            if (Guid.TryParse(payment.Metadata, out Guid reservationId))
            {
                await _reservationService.UpdatePaymentStatusAsync(
                    reservationId,
                    payment.Id,
                    ReservationpaymentStatus.Canceled
                );
            }
            Console.WriteLine($"Betaling geannuleerd: {payment.Metadata}");
        }

        return Ok();
    }
}
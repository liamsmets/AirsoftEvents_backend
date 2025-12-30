namespace AirsoftEvents.Api.Contracts;

public class StartPaymentResponseContract
{
    public string CheckoutUrl { get; set; } = "";
    public string PaymentId { get; set; } = "";
}

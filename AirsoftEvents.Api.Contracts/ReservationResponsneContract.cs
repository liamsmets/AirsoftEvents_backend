using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Api.Contracts;

public class ReservationResponseContract
{
    public Guid Id {get;set;}
    public Guid EventId {get;set;}
    public Guid UserId {get;set;}
    public DateTime ReservedAt {get;set;}

    public ReservationpaymentStatus PaymentStatus { get; set; }
    public string? MolliePaymentId { get; set; }
}
using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Persistance.Entities;

public class Reservation
{
    public Guid Id {get;set;}
    public Guid EventId {get;set;}
    public Guid UserId {get;set;}
    public DateTime ReservedAt {get;set;}
    public Event Event {get;set;} = null!;

    public ReservationpaymentStatus PaymentStatus {get;set;} = ReservationpaymentStatus.Pending;
    public string? MolliePaymentId {get;set;}
}
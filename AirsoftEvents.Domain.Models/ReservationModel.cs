using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Domain.Models;

public class ReservationModel
{
    public Guid Id {get;set;}
    public DateTime ReservedAt {get;set;} = DateTime.UtcNow;

    public Guid UserId{get;set;}
    public UserModel User {get;set;} = null!;

    public Guid EventId {get;set;}
    public EventModel Event {get;set;} = null!;

    public ReservationpaymentStatus paymentStatus {get;set;} = ReservationpaymentStatus.Pending;
    public string? MolliePaymentId {get;set;}
}
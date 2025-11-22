namespace AirsoftEvents.Core.Contracts;

public class ReservationResponseContract
{
    public Guid Id {get;set;}
    public Guid EventId {get;set;}
    public Guid UserId {get;set;}
    public DateTime ReservedAt {get;set;}
}
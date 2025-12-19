namespace AirsoftEvents.Api.Contracts;

public class ReservationRequestContract
{
    public Guid EventId {get;set;}
    public Guid UserId {get;set;}
}
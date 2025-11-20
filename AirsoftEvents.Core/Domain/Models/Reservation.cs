namespace AirsoftEvents.Core.Domain.Models;

public class Reservation
{
    public Guid Id {get;set;}
    public DateTime ReservedAt {get;set;} = DateTime.UtcNow;

    public Guid UserId{get;set;}
    public User User {get;set;} = null!;

    public Guid EventId {get;set;}
    public Event Event {get;set;} = null!;
}
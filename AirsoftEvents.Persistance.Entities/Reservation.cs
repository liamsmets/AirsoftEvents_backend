namespace AirsoftEvents.Persistance.Entities;

public class Reservation
{
    public Guid Id {get;set;}
    public Guid EventId {get;set;}
    public Guid UserId {get;set;}
    public DateTime ReservedAt {get;set;}

    public User owner {get;set;} = null!;
    public Event desevent {get;set;} = null!;
}
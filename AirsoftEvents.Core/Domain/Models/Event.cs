using AirsoftEvents.Core.Domain.Enums;
namespace AirsoftEvents.Core.Domain.Models;

public class Event
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public DateTime Date {get;set;}
    public decimal Price {get;set;}

    public int MaxPlayers {get;set;}

    public EventStatus Status { get; set; } = EventStatus.Pending;

    public Guid FieldId {get;set;}
    public AirsoftField Field {get;set;} = null!;

    public List<Reservation> Reservations {get;set;} = new();
}
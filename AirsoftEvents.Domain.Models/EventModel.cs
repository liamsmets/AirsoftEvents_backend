using AirsoftEvents.Domain.Models.Enums;
namespace AirsoftEvents.Domain.Models;

public class EventModel
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public DateTime Date {get;set;}
    public decimal Price {get;set;}

    public int MaxPlayers {get;set;}

    public EventStatus Status { get; set; } = EventStatus.Pending;

    public Guid FieldId {get;set;}
    public AirsoftFieldModel Field {get;set;} = null!;

    public List<ReservationModel> Reservations {get;set;} = new();
}
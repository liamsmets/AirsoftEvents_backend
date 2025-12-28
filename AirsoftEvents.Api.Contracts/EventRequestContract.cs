using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Api.Contracts;

public class EventRequestContract
{
    public string Name {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public DateTime Date {get;set;}
    public decimal Price {get;set;}
    public int MaxPlayers {get;set;}
    public Guid FieldId {get;set;}

    public Guid UserId {get;set;}
}
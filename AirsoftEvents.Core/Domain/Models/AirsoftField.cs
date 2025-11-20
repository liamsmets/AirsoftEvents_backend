using AirsoftEvents.Core.Domain.Enums;

namespace AirsoftEvents.Core.Domain.Models;

public class AirsoftField
{
    public Guid Id{get;set;}
    public string Name {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int Capacity {get;set;}

    public FieldStatus Status {get;set;} = FieldStatus.Pending;

    public Guid OwnerId{get;set;}
    public User Owner {get;set;} = null!;

    public List<Event> Events {get;set;} = new();
}
using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Domain.Models;

public class AirsoftFieldModel
{
    public Guid Id{get;set;}
    public string Name {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int Capacity {get;set;}

    public FieldStatus Status {get;set;} = FieldStatus.Pending;

    public Guid OwnerId{get;set;}
    public UserModel Owner {get;set;} = null!;

    public List<EventModel> Events {get;set;} = new();
}
using AirsoftEvents.Domain.Models.Enums;
namespace AirsoftEvents.Persistance.Entities;

public class Field
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int Capacity {get;set;}
    public required FieldStatus Status {get;set;}
    public Guid OwnerId{get;set;}

    public User owner {get;set;} = null!;
}
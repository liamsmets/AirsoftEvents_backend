namespace AirsoftEvents.Api.Contracts;

public class FieldRequestContract
{
    public string Name {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int Capacity {get;set;}
    public Guid OwnerId{get;set;}
}
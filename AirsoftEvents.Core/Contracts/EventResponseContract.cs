namespace AirsoftEvents.Core.Contracts;

public class EventResponseContract
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public DateTime Date {get;set;}
    public decimal Price {get;set;}
    public string Status {get;set;} = string.Empty;
    public Guid FieldId {get;set;}
}
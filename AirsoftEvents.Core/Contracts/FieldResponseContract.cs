namespace AirsoftEvents.Core.Contracts;

public class FieldResponseContract
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public int Capacity {get;set;}
    public string Status {get;set;} = string.Empty;
} 